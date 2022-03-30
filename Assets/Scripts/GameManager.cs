using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;

    public Text gameOverText;
    public Text restartText;
    public Text scoreText;
    public Text livesText;
    public Text highScoreText;

    public AudioSource audiosource;
    public AudioClip beginningSound;
    public AudioClip chompSound;
    public AudioClip deathSound;
    public AudioClip eatGhostSound;
    public int ghostMultiplier { get; private set; } = 1;
    public int score { get; private set; }
    public int highscore { get; private set; }
    public int lives { get; private set; }


    private void Start()
    {
        audiosource = GetComponent<AudioSource>();
        NewGame();
    }

    private void Update()
    {
        if ( this.lives <= 0 && Input.anyKeyDown)
        {
            NewGame();
        }
        
    }

    private void NewGame()
    {
        highscore = PlayerPrefs.GetInt("highscore", highscore);
        SetScore(0);
        SetLives(3);
        NewRound();
    }

    

    private void NewRound()
    {

        this.gameOverText.enabled = false;
        this.restartText.enabled = false;
        foreach(Transform pellet in this.pellets)
        {
            pellet.gameObject.SetActive(true);
        }
        audiosource.PlayOneShot(beginningSound);
        Thread.Sleep(4000);
        ResetState();
    }

    private void ResetState()
    {
        ResetGhostMultiplier();
        for (int i = 0; i <this.ghosts.Length; i++)
        {
            this.ghosts[i].ResetState();
        }
        
        
        this.pacman.ResetState();
    }
    
    private void GameOver()
    {
        this.gameOverText.enabled = true;
        this.restartText.enabled = true;
        if (score > highscore)
        {
            this.highscore = score;
            PlayerPrefs.SetInt("highscore", highscore);
            highScoreText.text = "HIGHSCORE : " + highscore;
        }
        
        
        for (int i = 0; i <this.ghosts.Length; i++)
        {
            this.ghosts[i].gameObject.SetActive(false);
        }

        this.pacman.gameObject.SetActive(false);
    }

    private void SetScore(int score)
    {
        this.score = score;
        this.scoreText.text = score.ToString().PadLeft(2, '0');
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        this.livesText.text = "x" + lives.ToString();
    }

    public void GhostEaten(Ghost ghost)
    {
        int points = ghost.points * this.ghostMultiplier;
        SetScore(this.score + points);
        this.ghostMultiplier++;
        audiosource.PlayOneShot(eatGhostSound);
    }

    public void PacmanEaten()
    {
        this.pacman.DeathSequence();
        audiosource.PlayOneShot(deathSound);
        SetLives(this.lives - 1);

        if (this.lives > 0)
        {
            //Will cast a function after x time (float) : here, we're calling 'ResetState' after 3sec
            Invoke(nameof(ResetState), 3.0f);
        } else {
            GameOver();
        }
    }

    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);
        SetScore(this.score + pellet.points);
        if (!audiosource.isPlaying)
        {
                    audiosource.PlayOneShot(chompSound);
        }
        if (!HasRemainingPellets()) {
            this.pacman.gameObject.SetActive(false);
            Invoke(nameof(NewRound), 3.0f);
        }
    }
    
    public void PowerPelletEaten(PowerPellet pellet)
    {
        foreach (var ghost in this.ghosts)
        {
            ghost.frightened.Enable(pellet.duration);
        }
        
        PelletEaten(pellet);
        CancelInvoke();
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);
    }

    private bool HasRemainingPellets()
    {
        foreach (Transform pellet in this.pellets)
        {
            if (pellet.gameObject.activeSelf) {
                return true;
            }
        }

        return false;
    }

    private void ResetGhostMultiplier()
    {
        this.ghostMultiplier = 1;
    }

    private void PlayAudioBeginning()
    {
        
    }

}

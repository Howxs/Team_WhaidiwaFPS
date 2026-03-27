using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;
using TMPro;
using UnityEngine.UI;

public class ZombieSpawnController : MonoBehaviour
{
    public int initialZombiePerWave = 5;
    public int currentZombiePerWave;

    public float spawnDelay = 0.5f;

    public int currentWave = 0;
    public float waveCooldown = 10f;

    public bool inCooldown;
    public float cooldownCounter = 0;

    public List<Enemy> currentZombiesAlive;

    public GameObject zombiePrefab;

    public TextMeshProUGUI waveOverUI;
    public TextMeshProUGUI cooldownCounterUI;

    public TextMeshProUGUI currentWaveUI;

    private void Start()
    {
        currentZombiePerWave = initialZombiePerWave;

        StartNextWave();
    }

    private void StartNextWave()
    {
        currentZombiesAlive.Clear();

        currentWave++;
        currentWaveUI.text = "WAVE : " + currentWave.ToString();

        StartCoroutine(SpawnWave());

    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < currentZombiePerWave; i++)
        {
            Vector3 spawnOffset = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
            Vector3 spawnPosition = transform.position + spawnOffset;

            var zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);

            Enemy enemyScript = zombie.GetComponent<Enemy>();

            currentZombiesAlive.Add(enemyScript);

            yield return new WaitForSeconds(spawnDelay);
        }
    }
    private void Update()
    {
       
        for (int i = currentZombiesAlive.Count - 1; i >= 0; i--)
        {
            if (currentZombiesAlive[i] == null || currentZombiesAlive[i].isDead)
            {
                currentZombiesAlive.RemoveAt(i);
            }
        }

        
        if (currentZombiesAlive.Count == 0 && inCooldown == false)
        {
            StartCoroutine(WaveCooldown());
        }

       
        if (inCooldown)
        {
            cooldownCounter -= Time.deltaTime;

            
            if (cooldownCounter < 0) cooldownCounter = 0;
        }
        else
        {
            cooldownCounter = waveCooldown;
        }

        cooldownCounterUI.text = cooldownCounter.ToString("F0");
    }

    private IEnumerator WaveCooldown()
    {
        inCooldown = true;
        waveOverUI.gameObject.SetActive(true);


        yield return new WaitForSeconds(waveCooldown);

        
        inCooldown = false;
        waveOverUI.gameObject.SetActive(false);

        
        currentZombiePerWave *= 2;

        
        StartNextWave();
    }
}

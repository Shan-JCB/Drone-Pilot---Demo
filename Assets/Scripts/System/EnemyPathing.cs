using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    //WaveConfig waveConfig;
    WaveConfig waveConfig;
    List<Transform> waypoints;
    int waypointIndex = 0;

    // Use this for initialization
    void Start()
    {
        // Do minimal work here. waveConfig may be assigned after Instantiate via SetWaveConfig.
        if (waveConfig != null)
        {
            InitializeFromConfig();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void SetWaveConfig(WaveConfig waveConfig)
    {
        this.waveConfig = waveConfig;
        InitializeFromConfig();
    }

    void InitializeFromConfig()
    {
        if (waveConfig == null)
        {
            Debug.LogWarning("EnemyPathing: SetWaveConfig called with null WaveConfig.");
            waypoints = new List<Transform>();
            return;
        }

        waypoints = waveConfig.GetWaypoints();
        waypointIndex = 0;
        if (waypoints != null && waypoints.Count > 0)
        {
            transform.position = waypoints[waypointIndex].transform.position;
        }
        else
        {
            Debug.LogWarning($"EnemyPathing: waveConfig '{waveConfig.name}' returned no waypoints; destroying enemy.");
            Destroy(gameObject);
        }
    }

    private void Move()
    {
        if (waypointIndex <= waypoints.Count - 1)
        {
            var targetPosition = waypoints[waypointIndex].transform.position;
            var movementThisFrame = waveConfig.GetMoveSpeed() * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);

            if (transform.position == targetPosition)
            {
                waypointIndex++;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

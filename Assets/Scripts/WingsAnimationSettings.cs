using System;
using UnityEngine;

[Serializable]
public class WingsAnimationSettings
{
	public float animationDuration = 1f;

	public float animationStartForce = 0.5f;

	public float animationEndForce = 1f;

	public float moveForce = 1000f;

	private readonly float startGlideTimeMin = 5f;

	private readonly float startGlideTimeMax = 8f;

	private readonly float glideDelayMin = 1f;

	private readonly float glideDelayMax = 3f;

	private float glideDelay;

	private float startGlideTime;

	private float startGlide;

	private bool gliding;

	private float startTime;

	private bool running;

	public bool CanAddForce
	{
		get
		{
			if ((Time.time - startTime) % animationDuration >= animationStartForce)
			{
				return (Time.time - startTime) % animationDuration < animationEndForce;
			}
			return false;
		}
	}

	public bool Glide
	{
		get
		{
			if (Time.time - startGlide >= glideDelay && gliding)
			{
				Begin();
				startGlide = 0f;
				gliding = false;
			}
			if (Time.time - startTime >= startGlideTime && !gliding)
			{
				startGlide = Time.time;
				gliding = true;
			}
			return gliding;
		}
	}

	public bool Running => running;

	public void Begin()
	{
		glideDelay = UnityEngine.Random.Range(glideDelayMin, glideDelayMax);
		startGlideTime = UnityEngine.Random.Range(startGlideTimeMin, startGlideTimeMax);
		startTime = Time.time;
		running = true;
	}

	public void End()
	{
		startGlide = 0f;
		gliding = false;
		running = false;
		startTime = 0f;
	}
}

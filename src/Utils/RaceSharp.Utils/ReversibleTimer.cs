using System.Diagnostics;
using System.Timers;
using Timer = System.Timers.Timer;

namespace RaceSharp;

public class ReversibleTimer
{
	private readonly Stopwatch _stopwatch;
	private readonly Timer _timer;

	public ReversibleTimer()
	{
		_stopwatch = new Stopwatch();
		_timer = new Timer();
		_timer.Elapsed += OnTimerElapsed;
	}

	public Action? OnElapsedTime { get; set; }
	public double CurrentInterval { get; private set; }
	public double RemainingTime { get; private set; }
	public double PassedTime { get; private set; }

	public bool IsGoingReversed { get; private set; }

	private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
	{
		_timer.Stop();
		OnElapsedTime?.Invoke();
	}

	private void ResetStopwatch()
	{
		_stopwatch.Reset();
		_stopwatch.Start();
	}

	public void Start(double intervalMilliseconds)
	{
		CurrentInterval = intervalMilliseconds;
		Stop();
		_timer.Start();
	}

	public void Resume()
	{
		if (IsGoingReversed)
		{
			if (PassedTime <= 0) return;

			_timer.Interval = PassedTime;
		}
		else
		{
			if (RemainingTime <= 0) return;

			_timer.Interval = RemainingTime;
		}

		ResetStopwatch();
		_timer.Start();
	}

	public void Reverse()
	{
		Pause();
		IsGoingReversed = !IsGoingReversed;
		Resume();
	}

	public void Pause()
	{
		_timer.Stop();
		_stopwatch.Stop();

		if (IsGoingReversed)
		{
			RemainingTime += _stopwatch.Elapsed.TotalMilliseconds;
			PassedTime -= _stopwatch.Elapsed.TotalMilliseconds;
		}
		else
		{
			RemainingTime -= _stopwatch.Elapsed.TotalMilliseconds;
			PassedTime += _stopwatch.Elapsed.TotalMilliseconds;
		}
	}

	public void Stop()
	{
		if (CurrentInterval == 0) return;

		_timer.Stop();
		_timer.Interval = CurrentInterval;

		PassedTime = 0;
		RemainingTime = CurrentInterval;

		ResetStopwatch();
	}
}
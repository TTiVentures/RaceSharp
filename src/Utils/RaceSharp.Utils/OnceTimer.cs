using Timer = System.Timers.Timer;

namespace RaceSharp;

public class OnceTimer : Timer {
	public OnceTimer() {
		AutoReset = false;
		Elapsed += (_, _) => Stop();
	}

	public void Start(double interval) {
		Interval = interval;
		Start();
	}
}
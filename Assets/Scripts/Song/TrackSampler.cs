using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TrackSampler<T> where T : Trackable {
    private List<T> stuff;
    private List<T> discarded;
    
    public double Time { get; set; }

    public TrackSampler(Track<T> track) {
        stuff = new List<T>(track.shit);
        discarded = new List<T>();
    }

    public void Update(double delta) {
        double newTime = Time + delta;

        for (int i = 0; i < stuff.Count; i++) {
            T trackable = stuff[i];
            if (trackable.time > newTime) break;

            discarded.Add(trackable);
            stuff.RemoveAt(i);
            i--;
        }

        Time = newTime;
    }

    public T[] Sample() {
        var result = discarded.ToArray();
        discarded.Clear();
        return result;
    }
}

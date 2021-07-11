using UnityEngine;

public static class NumericSpringing
{
    public static float Spring_Float(float value, ref float velocity, float target_value, float damping_ratio, float angular_frequency, float time_step)
    {
        //Math from: http://allenchou.net/2015/04/game-math-more-on-numeric-springing/ and http://allenchou.net/2015/04/game-math-precise-control-over-numeric-springing/
        float _x = value;                        //Input value
        float _v = velocity * (1f / time_step);  //Input velocity
        float _x_t = target_value;                       //Target value
        float _damping = damping_ratio;                  //Damping of the oscillation (0 = no damping, 1 = critically damped)
        float _ang_freq = 2 * Mathf.PI * angular_frequency;             //Oscillations per second
        float _t = time_step / 1f;              //How much of a second each step/use of the script takes (1 = normal time, 2 = twice as fast,..)
        float _delta_v, _delta_x, _delta;

        _delta = (1 + 2 * _t * _damping * _ang_freq) + Mathf.Pow(_t, 2) * Mathf.Pow(_ang_freq, 2);
        _delta_x = (1 + 2 * _t * _damping * _ang_freq) * _x + _t * _v + Mathf.Pow(_t, 2) * Mathf.Pow(_ang_freq, 2) * _x_t;
        _delta_v = _v + _t * Mathf.Pow(_ang_freq, 2) * (_x_t - _x);


        var ret = _delta_x / _delta;                        //Output value
        velocity = (_delta_v / _delta) / (1f / time_step); //Output velocity
        return ret;
    }
}
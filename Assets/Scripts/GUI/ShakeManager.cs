using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ShakeManager : MonoBehaviour
{
    public static ShakeManager singleton;

    public float amp, freq, t;

    private CinemachineVirtualCamera cmFreeCam;

    private void Start () {
        if (singleton != null) {
            print ("two shake managers on one scene. Destroing one..");
            Destroy (gameObject);
        }
        singleton = this;
        cmFreeCam = GetComponent<CinemachineVirtualCamera> ();
    }
    public void ShakeKatana () {
        StartCoroutine (InternalShakeKatana(amp, freq, t));
    }
    private IEnumerator InternalShakeKatana (float amplitudeGain, float frequencyGain, float time) {
        float timer = time;
        float startAmplitude = amplitudeGain;
        while(timer > time / 2) {
            timer -= Time.deltaTime;

            Noise (amplitudeGain, frequencyGain);
            amplitudeGain = Mathf.Lerp (0, startAmplitude, 1 - (timer - (time / 2)) / (time / 2));

            yield return new WaitForEndOfFrame ();
        }
        amplitudeGain = startAmplitude;
        while (timer > 0) {
            timer -= Time.deltaTime;

            Noise (amplitudeGain, frequencyGain);
            amplitudeGain = Mathf.Lerp (startAmplitude, 0, ((time / 2) - timer) / (time / 2));

            yield return new WaitForEndOfFrame ();
        }
        amplitudeGain = 0;
        Noise (amplitudeGain, frequencyGain);
    }
    public void Noise (float amplitudeGain, float frequencyGain) {
        CinemachineBasicMultiChannelPerlin noise =
            cmFreeCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin> ();

        noise.m_AmplitudeGain = amplitudeGain;
        noise.m_FrequencyGain = frequencyGain;

    }
}

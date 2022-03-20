using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.TestTools;

public class audio_source {

    [UnityTest]
    public IEnumerator loops() {
        
        // Setup
        var testObj = new GameObject();
        AudioSource audio = testObj.AddComponent<AudioSource>();
        audio.clip = Resources.Load<AudioClip>("Audio/ShortDrumLoop");
        
        // Action
        audio.loop = true;
        audio.Play();

        yield return new WaitForSeconds(audio.clip.length + .1f);

        // Test whether audio playes after its duration
        Assert.AreEqual(true, audio.isPlaying);

        // Test whether it loops at least a couple of times
        yield return new WaitForSeconds(audio.clip.length);
        Assert.AreEqual(true, audio.isPlaying);

        yield return null;
    }

    [UnityTest]
    public IEnumerator matches_clip_volume() {

        // Setup
        var testObj = new GameObject();
        AudioSource audio = testObj.AddComponent<AudioSource>();
        audio.clip = Resources.Load<AudioClip>("Audio/MusicLoop");
        int numSamples = 256; // Must be a power of 2
        int numChecks = 3; // number of samples to test
        float volume = .6f;

        // Action
        audio.volume = volume;
        audio.loop = true;
        audio.Play();

        // Get output
        float[] actualOutput = new float[numSamples];
        audio.GetOutputData(actualOutput, 1);

        yield return new WaitForSeconds(.1f);

        int currSample = audio.timeSamples;
        audio.GetOutputData(actualOutput, 1);

        float[] samples = new float[numSamples];
        audio.clip.GetData(samples, currSample - numSamples);

        // Extract left channel from stereo track
        float[] samples_left = new float[numSamples/2];
        for (int i = 0; i < samples_left.Length; i++) {
            samples_left[i] = samples[i*2];
        }

        // Test 
        for (int i = 0; i < numChecks; i++) {
            Assert.AreEqual(samples_left[i] * volume, actualOutput[i]);
        }

        yield return null;

    }
}

using System;
using System.Threading;
using System.IO;
using MiniAudioEx.Core.StandardAPI;
using MiniAudioEx.Native;
using NFluidsynth;

var settings = new Settings();
var synth = new Synth(settings);
var soundfont = Path.Join(Environment.CurrentDirectory, "Grand Piano.sf3");

AudioContext.Initialize(44100, 2, 2, 512);

synth.LoadSoundFont(soundfont, true);

var audioSource = new AudioSource();
audioSource.Read += (NativeArray<float> framesOut, ulong frameCount, int channels) =>
{
    if (synth.Disposed) return;
    var tempBuffer = new Span<float>(new float[framesOut.Length]);
    synth.WriteSampleFloat(tempBuffer.Length / channels, tempBuffer, 0, 2, tempBuffer, 1, 2);
    for (int i = 0; i < framesOut.Length; i++)
    {
        framesOut[i] = tempBuffer[i];
    }
};
audioSource.Play();
synth.NoteOn(0, 60, 120);
Thread.Sleep(5000);
synth.NoteOff(0, 60);
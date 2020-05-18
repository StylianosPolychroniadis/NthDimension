using NthDimension.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Rasterizer
{
    public abstract class AudioBase
    {
        public string DeviceName;
        public virtual void Create() { }
        public virtual void Destroy() { }
        public virtual void BufferData<TBuffer>(int bid, ALFormat format, TBuffer[] buffer, int size, int freq) where TBuffer : struct { }
        //public virtual void BufferData(int bid, ALFormat format, IntPtr buffer, int size, int freq) { }
        public virtual void Source(int sid, ALSourcei param, int value) { }
        public virtual void Source(int sid, ALSourceb param, bool value) { }
        public virtual void Source(int sid, ALSourcef param, float value) { }
        public virtual void Source(int sid, ALSource3f param, float value1, float value2, float value3) { }
        public virtual void Listener(ALListener3f param, Vector3 vector) { }
        public virtual void Listener(ALListener3f param, float value1, float value2, float value3) { }
        public virtual int GenBuffer() { return -1; }
        public virtual void DeleteBuffer(int buffer) { }
        public virtual void DeleteSource(int source) { }
        public virtual int GenSource() { return -1; }

        public virtual void SourceStop(int sid) { }
        public virtual void SourcePlay(int sid) { }
        public virtual void SourcePause(int sid) { }

        public virtual void GetSource(int sid, ALGetSourcei param, out int value) { value = -1; }
        public virtual void SourceQueueBuffer(int source, int buffer) { }
        public virtual void SourceUnqueueBuffers(int sid, int numEntries, int[] bids) { }

        public virtual void MakeCurrentContext()
        {

        }
        ///// <summary>
        ///// Completely wrong, stuck at audio replay
        ///// </summary>
        //public virtual void Update() { }

        public enum ALFormat
        {
            //
            // Summary:
            //     1 Channel, 8 bits per sample.
            Mono8 = 4352,
            //
            // Summary:
            //     1 Channel, 16 bits per sample.
            Mono16 = 4353,
            //
            // Summary:
            //     2 Channels, 8 bits per sample each.
            Stereo8 = 4354,
            //
            // Summary:
            //     2 Channels, 16 bits per sample each.
            Stereo16 = 4355,
            //
            // Summary:
            //     Multichannel 4.0, 8-bit data. Requires Extension: AL_EXT_MCFORMATS
            MultiQuad8Ext = 4612,
            //
            // Summary:
            //     Multichannel 4.0, 16-bit data. Requires Extension: AL_EXT_MCFORMATS
            MultiQuad16Ext = 4613,
            //
            // Summary:
            //     Multichannel 4.0, 32-bit data. Requires Extension: AL_EXT_MCFORMATS
            MultiQuad32Ext = 4614,
            //
            // Summary:
            //     1 Channel rear speaker, 8-bit data. See Quadrophonic setups. Requires Extension:
            //     AL_EXT_MCFORMATS
            MultiRear8Ext = 4615,
            //
            // Summary:
            //     1 Channel rear speaker, 16-bit data. See Quadrophonic setups. Requires Extension:
            //     AL_EXT_MCFORMATS
            MultiRear16Ext = 4616,
            //
            // Summary:
            //     1 Channel rear speaker, 32-bit data. See Quadrophonic setups. Requires Extension:
            //     AL_EXT_MCFORMATS
            MultiRear32Ext = 4617,
            //
            // Summary:
            //     Multichannel 5.1, 8-bit data. Requires Extension: AL_EXT_MCFORMATS
            Multi51Chn8Ext = 4618,
            //
            // Summary:
            //     Multichannel 5.1, 16-bit data. Requires Extension: AL_EXT_MCFORMATS
            Multi51Chn16Ext = 4619,
            //
            // Summary:
            //     Multichannel 5.1, 32-bit data. Requires Extension: AL_EXT_MCFORMATS
            Multi51Chn32Ext = 4620,
            //
            // Summary:
            //     Multichannel 6.1, 8-bit data. Requires Extension: AL_EXT_MCFORMATS
            Multi61Chn8Ext = 4621,
            //
            // Summary:
            //     Multichannel 6.1, 16-bit data. Requires Extension: AL_EXT_MCFORMATS
            Multi61Chn16Ext = 4622,
            //
            // Summary:
            //     Multichannel 6.1, 32-bit data. Requires Extension: AL_EXT_MCFORMATS
            Multi61Chn32Ext = 4623,
            //
            // Summary:
            //     Multichannel 7.1, 8-bit data. Requires Extension: AL_EXT_MCFORMATS
            Multi71Chn8Ext = 4624,
            //
            // Summary:
            //     Multichannel 7.1, 16-bit data. Requires Extension: AL_EXT_MCFORMATS
            Multi71Chn16Ext = 4625,
            //
            // Summary:
            //     Multichannel 7.1, 32-bit data. Requires Extension: AL_EXT_MCFORMATS
            Multi71Chn32Ext = 4626,
            //
            // Summary:
            //     1 Channel, IMA4 ADPCM encoded data. Requires Extension: AL_EXT_IMA4
            MonoIma4Ext = 4864,
            //
            // Summary:
            //     2 Channels, IMA4 ADPCM encoded data. Requires Extension: AL_EXT_IMA4
            StereoIma4Ext = 4865,
            //
            // Summary:
            //     Ogg Vorbis encoded data. Requires Extension: AL_EXT_vorbis
            VorbisExt = 65539,
            //
            // Summary:
            //     1 Channel, single-precision floating-point data. Requires Extension: AL_EXT_float32
            MonoFloat32Ext = 65552,
            //
            // Summary:
            //     2 Channels, single-precision floating-point data. Requires Extension: AL_EXT_float32
            StereoFloat32Ext = 65553,
            //
            // Summary:
            //     1 Channel, double-precision floating-point data. Requires Extension: AL_EXT_double
            MonoDoubleExt = 65554,
            //
            // Summary:
            //     2 Channels, double-precision floating-point data. Requires Extension: AL_EXT_double
            StereoDoubleExt = 65555,
            //
            // Summary:
            //     1 Channel, µ-law encoded data. Requires Extension: AL_EXT_MULAW
            MonoMuLawExt = 65556,
            //
            // Summary:
            //     2 Channels, µ-law encoded data. Requires Extension: AL_EXT_MULAW
            StereoMuLawExt = 65557,
            //
            // Summary:
            //     1 Channel, A-law encoded data. Requires Extension: AL_EXT_ALAW
            MonoALawExt = 65558,
            //
            // Summary:
            //     2 Channels, A-law encoded data. Requires Extension: AL_EXT_ALAW
            StereoALawExt = 65559,
            //
            // Summary:
            //     MP3 encoded data. Requires Extension: AL_EXT_mp3
            Mp3Ext = 65568
        }
        public enum ALGetSourcei
        {
            //
            // Summary:
            //     Indicate the Buffer to provide sound samples. Type: uint Range: any valid Buffer
            //     Handle.
            Buffer = 4105,
            //
            // Summary:
            //     The state of the source (Stopped, Playing, etc.) Use the enum AlSourceState for
            //     comparison.
            SourceState = 4112,
            //
            // Summary:
            //     The number of buffers queued on this source.
            BuffersQueued = 4117,
            //
            // Summary:
            //     The number of buffers in the queue that have been processed.
            BuffersProcessed = 4118,
            //
            // Summary:
            //     The playback position, expressed in samples. AL_EXT_OFFSET Extension.
            SampleOffset = 4133,
            //
            // Summary:
            //     The playback position, expressed in bytes. AL_EXT_OFFSET Extension.
            ByteOffset = 4134,
            //
            // Summary:
            //     Source type (Static, Streaming or undetermined). Use enum AlSourceType for comparison.
            SourceType = 4135
        }
        public enum ALSourceState
        {
            //
            // Summary:
            //     Default State when loaded, can be manually set with AL.SourceRewind().
            Initial = 4113,
            //
            // Summary:
            //     The source is currently playing.
            Playing = 4114,
            //
            // Summary:
            //     The source has paused playback.
            Paused = 4115,
            //
            // Summary:
            //     The source is not playing.
            Stopped = 4116
        }
        //
        // Summary:
        //     A list of valid Int32 Source parameters
        public enum ALSourcei
        {
            //
            // Summary:
            //     Indicate the Buffer to provide sound samples. Type: uint Range: any valid Buffer
            //     Handle.
            Buffer = 4105,
            //
            // Summary:
            //     The playback position, expressed in samples.
            SampleOffset = 4133,
            //
            // Summary:
            //     The playback position, expressed in bytes.
            ByteOffset = 4134,
            //
            // Summary:
            //     Source type (Static, Streaming or undetermined). Use enum AlSourceType for comparison
            SourceType = 4135,
            //
            // Summary:
            //     (EFX Extension) This Source property is used to apply filtering on the direct-path
            //     (dry signal) of a Source.
            EfxDirectFilter = 131077
        }
        //
        // Summary:
        //     A list of valid 8-bit boolean Source/GetSource parameters
        public enum ALSourceb
        {
            //
            // Summary:
            //     Indicate that the Source has relative coordinates. Type: bool Range: [True, False]
            SourceRelative = 514,
            //
            // Summary:
            //     Indicate whether the Source is looping. Type: bool Range: [True, False] Default:
            //     False.
            Looping = 4103,
            //
            // Summary:
            //     (EFX Extension) If this Source property is set to True, this Source’s direct-path
            //     is automatically filtered according to the orientation of the source relative
            //     to the listener and the setting of the Source property Sourcef.ConeOuterGainHF.
            //     Type: bool Range [False, True] Default: True
            EfxDirectFilterGainHighFrequencyAuto = 131082,
            //
            // Summary:
            //     (EFX Extension) If this Source property is set to True, the intensity of this
            //     Source’s reflected sound is automatically attenuated according to source-listener
            //     distance and source directivity (as determined by the cone parameters). If it
            //     is False, the reflected sound is not attenuated according to distance and directivity.
            //     Type: bool Range [False, True] Default: True
            EfxAuxiliarySendFilterGainAuto = 131083,
            //
            // Summary:
            //     (EFX Extension) If this Source property is AL_TRUE (its default value), the intensity
            //     of this Source’s reflected sound at high frequencies will be automatically attenuated
            //     according to the high-frequency source directivity as set by the Sourcef.ConeOuterGainHF
            //     property. If this property is AL_FALSE, the Source’s reflected sound is not filtered
            //     at all according to the Source’s directivity. Type: bool Range [False, True]
            //     Default: True
            EfxAuxiliarySendFilterGainHighFrequencyAuto = 131084
        }
        //
        // Summary:
        //     A list of valid 32-bit Float Source/GetSource parameters
        public enum ALSourcef
        {
            //
            // Summary:
            //     Directional Source, inner cone angle, in degrees. Range: [0-360] Default: 360
            ConeInnerAngle = 4097,
            //
            // Summary:
            //     Directional Source, outer cone angle, in degrees. Range: [0-360] Default: 360
            ConeOuterAngle = 4098,
            //
            // Summary:
            //     Specify the pitch to be applied, either at Source, or on mixer results, at Listener.
            //     Range: [0.5f - 2.0f] Default: 1.0f
            Pitch = 4099,
            //
            // Summary:
            //     Indicate the gain (volume amplification) applied. Type: float. Range: [0.0f -
            //     ? ] A value of 1.0 means un-attenuated/unchanged. Each division by 2 equals an
            //     attenuation of -6dB. Each multiplicaton with 2 equals an amplification of +6dB.
            //     A value of 0.0f is meaningless with respect to a logarithmic scale; it is interpreted
            //     as zero volume - the channel is effectively disabled.
            Gain = 4106,
            //
            // Summary:
            //     Indicate minimum Source attenuation. Type: float Range: [0.0f - 1.0f] (Logarthmic)
            MinGain = 4109,
            //
            // Summary:
            //     Indicate maximum Source attenuation. Type: float Range: [0.0f - 1.0f] (Logarthmic)
            MaxGain = 4110,
            //
            // Summary:
            //     Source specific reference distance. Type: float Range: [0.0f - float.PositiveInfinity]
            //     At 0.0f, no distance attenuation occurs. Type: float Default: 1.0f.
            ReferenceDistance = 4128,
            //
            // Summary:
            //     Source specific rolloff factor. Type: float Range: [0.0f - float.PositiveInfinity]
            RolloffFactor = 4129,
            //
            // Summary:
            //     Directional Source, outer cone gain. Default: 0.0f Range: [0.0f - 1.0] (Logarithmic)
            ConeOuterGain = 4130,
            //
            // Summary:
            //     Indicate distance above which Sources are not attenuated using the inverse clamped
            //     distance model. Default: float.PositiveInfinity Type: float Range: [0.0f - float.PositiveInfinity]
            MaxDistance = 4131,
            //
            // Summary:
            //     The playback position, expressed in seconds.
            SecOffset = 4132,
            //
            // Summary:
            //     (EFX Extension) This property is a multiplier on the amount of Air Absorption
            //     applied to the Source. The AL_AIR_ABSORPTION_FACTOR is multiplied by an internal
            //     Air Absorption Gain HF value of 0.994 (-0.05dB) per meter which represents normal
            //     atmospheric humidity and temperature. Range [0.0f .. 10.0f] Default: 0.0f
            EfxAirAbsorptionFactor = 131079,
            //
            // Summary:
            //     (EFX Extension) This property is defined the same way as the Reverb Room Rolloff
            //     property: it is one of two methods available in the Effect Extension to attenuate
            //     the reflected sound (early reflections and reverberation) according to source-listener
            //     distance. Range [0.0f .. 10.0f] Default: 0.0f
            EfxRoomRolloffFactor = 131080,
            //
            // Summary:
            //     (EFX Extension) A directed Source points in a specified direction. The Source
            //     sounds at full volume when the listener is directly in front of the source; it
            //     is attenuated as the listener circles the Source away from the front. Range [0.0f
            //     .. 1.0f] Default: 1.0f
            EfxConeOuterGainHighFrequency = 131081
        }
        //
        // Summary:
        //     A list of valid Math.Vector3 Source/GetSource parameters
        public enum ALSource3f
        {
            //
            // Summary:
            //     Specify the current location in three dimensional space. OpenAL, like OpenGL,
            //     uses a right handed coordinate system, where in a frontal default view X (thumb)
            //     points right, Y points up (index finger), and Z points towards the viewer/camera
            //     (middle finger). To switch from a left handed coordinate system, flip the sign
            //     on the Z coordinate. Listener position is always in the world coordinate system.
            Position = 4100,
            //
            // Summary:
            //     Specify the current direction vector.
            Direction = 4101,
            //
            // Summary:
            //     Specify the current velocity in three dimensional space.
            Velocity = 4102
        }
        //
        // Summary:
        //     A list of valid Math.Vector3 Listener/GetListener parameters
        public enum ALListener3f
        {
            //
            // Summary:
            //     Specify the current location in three dimensional space. OpenAL, like OpenGL,
            //     uses a right handed coordinate system, where in a frontal default view X (thumb)
            //     points right, Y points up (index finger), and Z points towards the viewer/camera
            //     (middle finger). To switch from a left handed coordinate system, flip the sign
            //     on the Z coordinate. Listener position is always in the world coordinate system.
            Position = 4100,
            //
            // Summary:
            //     Specify the current velocity in three dimensional space.
            Velocity = 4102
        }

    }
}

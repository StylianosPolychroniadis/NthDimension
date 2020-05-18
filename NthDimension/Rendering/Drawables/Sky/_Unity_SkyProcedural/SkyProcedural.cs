using NthDimension.Rendering.Scenegraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Rendering.Drawables.Models
{
	using System.Collections;
    using System.Drawing;
    using NthDimension.Algebra;
    using NthDimension.Rendering.Scenegraph;

	public class SkyProcedural : Skybox
	{

		//[Header("Light")]
		//[SerializeField] 
		private Lights.Light directionalLight;

		//[Header("Time/World")]
		//[SerializeField] 
		private AnimationCurve worldLongitudeCurve = AnimationCurve.Linear(0, 25f, 1f, 25f);
		//[SerializeField] 
		private float dayLengthInSeconds = 600f;
		//[SerializeField] [Range(0f, 24f)] 
		private float sunriseTime;
		//[SerializeField] [Range(0f, 24f)] 
		private float sunsetTime;

		//[Header("Sun")]
		//[SerializeField] 
		private AnimationCurve sunSizeCurve = AnimationCurve.Linear(0f, .07f, 1f, .07f);
		//[SerializeField]
		private Gradient sunColorGradient = new Gradient()
		{
			colorKeys = new GradientColorKey[]
			{
			new GradientColorKey(new Color(1f, .639f, .482f, 1f), .25f),
			new GradientColorKey(new Color(1f, .725f, .482f, 1f), .30f),
			new GradientColorKey(new Color(1f, .851f, .722f, 1f), .50f),
			new GradientColorKey(new Color(1f, .725f, .482f, 1f), .70f),
			new GradientColorKey(new Color(1f, .639f, .482f, 1f), .75f)
			},

			alphaKeys = new GradientAlphaKey[]
			{
			new GradientAlphaKey(1f, 0f),
			new GradientAlphaKey(1f,1f)
			}
		};

		//[SerializeField]
		private AnimationCurve sunintensityCurve = new AnimationCurve()
		{
			keys = new Keyframe[]
			{
			new Keyframe( 0f,0f),
			new Keyframe( .25f,0f),
			new Keyframe( .30f,1f),
			new Keyframe( .70f,1f),
			new Keyframe( .75f,0f),
			new Keyframe( 1f,0f)
			}
		};

		//[Header("Ambient/Atmosphere")]
		//[SerializeField] 
		private AMBIENTMODE ambientMode = AMBIENTMODE.GRADIENT;
		//[SerializeField] 
		private AnimationCurve exposureCurve = AnimationCurve.Linear(0f, 1.3f, 0f, 1.3f);
		//[SerializeField] 
		private AnimationCurve atmosphereThicknessCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
		[SerializeField]
		private Gradient ambientSkycolorGradient = new Gradient()
		{
			colorKeys = new GradientColorKey[]
			{
			new GradientColorKey(new Color( .004f, .071f, .161f, 1f), .22f),
			new GradientColorKey(new Color( .435f, .494f, .498f, 1f), .25f),
			new GradientColorKey(new Color( .463f, .576f, .769f, 1f), .30f),
			new GradientColorKey(new Color( .463f, .576f, .769f, 1f), .70f),
			new GradientColorKey(new Color( .435f, .494f, .498f, 1f), .75f),
			new GradientColorKey(new Color( .004f, .071f, .161f, 1f), .78f)

			},

			alphaKeys = new GradientAlphaKey[]
			{
			new GradientAlphaKey(1f, .20f),
			new GradientAlphaKey(1f, .70f)
			}
		};

		//[SerializeField]
		private Gradient ambientEquatorcolorGradient = new Gradient()
		{
			colorKeys = new GradientColorKey[]
			{
			new GradientColorKey(new Color( .008f, .125f, .275f, 1f), .22f),
			new GradientColorKey(new Color( .859f, .780f, .561f, 1f), .25f),
			new GradientColorKey(new Color( .698f, .843f,    1f, 1f), .30f),
			new GradientColorKey(new Color( .698f, .843f,    1f, 1f), .70f),
			new GradientColorKey(new Color( .859f, .780f, .561f, 1f), .75f),
			new GradientColorKey(new Color( .008f, .125f, .275f, 1f), .78f)

			},

			alphaKeys = new GradientAlphaKey[]
			{
			new GradientAlphaKey(1f, .20f),
			new GradientAlphaKey(1f, .70f)
			}
		};
		//[SerializeField]
		private Gradient ambientGroundcolorGradient = new Gradient()
		{
			colorKeys = new GradientColorKey[]
			{
			new GradientColorKey(new Color(    0f,    0f,    0f, 1f), .22f),
			new GradientColorKey(new Color( .227f, .157f, .102f, 1f), .25f),
			new GradientColorKey(new Color( .467f, .435f, .416f, 1f), .30f),
			new GradientColorKey(new Color( .467f, .435f, .416f, 1f), .70f),
			new GradientColorKey(new Color( .227f, .157f, .102f, 1f), .75f),
			new GradientColorKey(new Color(    0f,    0f,    0f, 1f), .78f)

			},

			alphaKeys = new GradientAlphaKey[]
			{
			new GradientAlphaKey(1f, .20f),
			new GradientAlphaKey(1f, .70f)
			}
		};

		//[SerializeField] 
		private AnimationCurve ambientIntensityCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		//[Header("Fog")]
		//[SerializeField] private bool useFog = false;
		//[SerializeField] private FOGTYPE fogtype = FOGTYPE.RENDERSETTINGS;
		//[SerializeField] private FogMode fogmode = FogMode.ExponentialSquared;
		//[SerializeField] private AnimationCurve fogDensityCurve = AnimationCurve.Linear(0, .0016f, 1f, .0016f);
		//[SerializeField] private AnimationCurve fogStartDistanceCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);
		//[SerializeField] private AnimationCurve fogEndDistanceCurve = AnimationCurve.Linear(0f, 300f, 1f, 300f);
		//[SerializeField]
		private Gradient fogcolorGradient = new Gradient()
		{
			colorKeys = new GradientColorKey[]
			{
			new GradientColorKey(new Color(    0f,    0f,    0f, 1f), .22f),
			new GradientColorKey(new Color( .227f, .157f, .102f, 1f), .25f),
			new GradientColorKey(new Color( .467f, .435f, .416f, 1f), .30f),
			new GradientColorKey(new Color( .467f, .435f, .416f, 1f), .70f),
			new GradientColorKey(new Color( .227f, .157f, .102f, 1f), .75f),
			new GradientColorKey(new Color(    0f,    0f,    0f, 1f), .78f)

			},

			alphaKeys = new GradientAlphaKey[]
			{
			new GradientAlphaKey(1f, .20f),
			new GradientAlphaKey(1f, .70f)
			}
		};


		protected Material skyMaterial;
		protected float worldLongitude;
		protected float currentTime;
		protected float sunSize;
		protected Color sunColor;
		protected float sunIntensity;
		protected Color ambientSkyColor;
		protected Color ambientEquatorColor;
		protected Color ambientGroundColor;
		protected float ambientIntensity;
		protected float atmosphereThickness;
		protected float exposure;
		protected float fogDensity;
		protected float fogStartDistance;
		protected float fogEndDistance;
		protected Color fogColor;



		public float WorldLongitude
		{
			get { return worldLongitude; }
			set { worldLongitude = value; }
		}

		public Vector3 WorldRotation { get; private set; }
		public float Hour { get; private set; }
		public float Minute { get; private set; }
		public float ProgressedTime { get { return (currentTime / 24); } }

		public bool IsDay;
		public bool IsNight;

		public float SunSize
		{
			get { return sunSize; }
			set { sunSize = value; }
		}
		public Color SunColor
		{
			get { return sunColor; }
			set { sunColor = value; }
		}

		public float SunLightIntensity
		{
			get { return sunIntensity; }
			set { sunIntensity = value; }
		}

		public float Exposure
		{
			get { return exposure; }
			set { exposure = value; }
		}

		public Color AmbientSkycolor
		{
			get { return ambientSkyColor; }
			set { ambientSkyColor = value; }
		}

		public Color AmbientEquatorColor
		{
			get { return ambientEquatorColor; }
			set { ambientEquatorColor = value; }
		}

		public Color AmbientGroundColor
		{
			get { return ambientGroundColor; }
			set { ambientGroundColor = value; }
		}

		public float AmbientIntensity
		{
			get { return ambientIntensity; }
			set { ambientIntensity = value; }
		}

		public float AtmosphereThickness
		{
			get { return atmosphereThickness; }
			set { atmosphereThickness = value; }
		}

		public float FogDensity
		{
			get { return fogDensity; }
			set { fogDensity = value; }
		}

		public float FogStartDistance
		{
			get { return fogStartDistance; }
			set { fogStartDistance = value; }
		}

		public float FogEndDistance
		{
			get { return fogEndDistance; }
			set { fogEndDistance = value; }
		}

		public Color FogColor
		{
			get { return fogColor; }
			set { fogColor = value; }
		}

		protected enum AMBIENTMODE { COLOR, GRADIENT, SKYBOX };
		protected enum FOGTYPE { RENDERSETTINGS, EVALUATEONLY, OFF };


		private void Start()
		{
			////Set the sky material
			//skyMaterial = RenderSettings.skybox;
		}

		private void Update()
		{
			UpdateTime();
			UpdateWorldRotation();
			UpdateSun();
			UpdateFog();

			if (ambientMode == AMBIENTMODE.SKYBOX)
			{
				UpdateSkybox();
			}
			else
			{
				UpdateAmbient();
			}
		}

		private void UpdateTime()
		{
			//Counting time
			currentTime += (Time.deltaTime / dayLengthInSeconds) * 24f;

			Hour = (float)Math.Floor(currentTime);
			Minute = (float)Math.Floor((currentTime - Hour) * 60);

			//Prevent the currentTime to exceed the day duration
			if (currentTime > 24)
			{
				currentTime = 0;
			}

			if (currentTime < 0)
			{
				currentTime = 24;
			}

			//Determin if Day or Night
			if (currentTime <= sunriseTime || currentTime >= sunsetTime)
			{
				IsDay = false;
				IsNight = true;
			}
			else
			{
				IsDay = true;
				IsNight = false;
			}
		}

		private void UpdateWorldRotation()
		{
			//Get longitude
			WorldLongitude = worldLongitudeCurve.Evaluate(ProgressedTime) - 90f;

			WorldRotation = new Vector3()
			{
				x = currentTime * (360f / 24f) - 90f,
				y = WorldLongitude,
				z = 0f
			};
		}

		private void UpdateSun()
		{
			//Evaluate Sun curves and gradients
			SunLightIntensity = sunintensityCurve.Evaluate(ProgressedTime);
			SunColor = sunColorGradient.Evaluate(ProgressedTime);

			//Updating the directional light
			directionalLight.transform.localEulerAngles = new Vector3()
			{
				x = WorldRotation.x,
				y = WorldRotation.y,
				z = 0f
			};

			directionalLight.transform.parent = this.transform;
			directionalLight.intensity = SunLightIntensity;
			directionalLight.color = sunColor;
		}

		private void UpdateAmbient()
		{
			//Evaluate Ambient curves and Gradients
			AmbientSkycolor = ambientSkycolorGradient.Evaluate(ProgressedTime);
			AmbientEquatorColor = ambientEquatorcolorGradient.Evaluate(ProgressedTime);
			AmbientGroundColor = ambientGroundcolorGradient.Evaluate(ProgressedTime);
			AmbientIntensity = ambientIntensityCurve.Evaluate(ProgressedTime);

			//Update RenderSettings according to ambient mode
			switch (ambientMode)
			{
				case AMBIENTMODE.SKYBOX:
					RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
					RenderSettings.ambientIntensity = AmbientIntensity;
					break;
				case AMBIENTMODE.COLOR:
					RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
					RenderSettings.ambientSkyColor = AmbientSkycolor;
					break;
				case AMBIENTMODE.GRADIENT:
					RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
					RenderSettings.ambientSkyColor = AmbientSkycolor;
					RenderSettings.ambientEquatorColor = AmbientEquatorColor;
					RenderSettings.ambientGroundColor = AmbientGroundColor;
					break;
			}
		}

		private void UpdateFog()
		{
			if (fogtype == FOGTYPE.OFF && fogtype != FOGTYPE.EVALUATEONLY)
			{
				return;
			}

			////Evaluate fog curves and gradients
			//FogDensity = fogDensityCurve.Evaluate(ProgressedTime);
			//FogStartDistance = fogStartDistanceCurve.Evaluate(ProgressedTime);
			//FogEndDistance = fogEndDistanceCurve.Evaluate(ProgressedTime);
			//FogColor = fogcolorGradient.Evaluate(ProgressedTime);

			//if (fogtype == FOGTYPE.RENDERSETTINGS)
			//{
			//	RenderSettings.fog = useFog;
			//	RenderSettings.fogMode = fogmode;
			//	RenderSettings.fogColor = FogColor;
			//}

			////Adjusting Fog to curves and gradients
			//switch (fogmode)
			//{
			//	case FogMode.Exponential:
			//		RenderSettings.fogDensity = FogDensity;
			//		break;
			//	case FogMode.ExponentialSquared:
			//		RenderSettings.fogDensity = FogDensity;
			//		break;
			//	case FogMode.Linear:
			//		RenderSettings.fogStartDistance = FogStartDistance;
			//		RenderSettings.fogEndDistance = FogEndDistance;
			//		break;
			//}
		}

		private void UpdateSkybox()
		{
			//Evaluate curves and gradients
			AmbientSkycolor = ambientSkycolorGradient.Evaluate(ProgressedTime);
			AtmosphereThickness = atmosphereThicknessCurve.Evaluate(ProgressedTime);
			AmbientGroundColor = ambientGroundcolorGradient.Evaluate(ProgressedTime);
			Exposure = exposureCurve.Evaluate(ProgressedTime);
			SunColor = sunColorGradient.Evaluate(ProgressedTime);
			SunSize = sunSizeCurve.Evaluate(ProgressedTime);

			//Adjusting Skybox
			skyMaterial.SetFloat("_Exposure", Exposure);
			skyMaterial.SetFloat("_AtmosphereThickness", AtmosphereThickness);
			skyMaterial.SetColor("_SkyTint", AmbientSkycolor);
			skyMaterial.SetColor("_GroundColor", AmbientGroundColor);
			skyMaterial.SetFloat("_SunSize", SunSize);
		}
	}

}

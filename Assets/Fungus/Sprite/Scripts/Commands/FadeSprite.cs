using UnityEngine;
using UnityEngine.Serialization;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Sprite", 
	             "Fade Sprite", 
	             "Fades a sprite to a target color over a period of time.")]
	[AddComponentMenu("")]
	public class FadeSprite : Command, ISerializationCallbackReceiver 
	{
		[Tooltip("Sprite object to be faded")]
		public SpriteRenderer spriteRenderer;

		[Tooltip("Length of time to perform the fade")]
		public FloatData _duration = new FloatData(1f);

		[Tooltip("Target color to fade to. To only fade transparency level, set the color to white and set the alpha to required transparency.")]
		public ColorData _targetColor = new ColorData(Color.white);

		[Tooltip("Wait until the fade has finished before executing the next command")]
		public bool waitUntilFinished = true;

		public override void OnEnter()
		{
			if (spriteRenderer == null)
			{
				Continue();
				return;
			}

			CameraController cameraController = CameraController.GetInstance();

			if (waitUntilFinished)
			{
				cameraController.waiting = true;
			}

			SpriteFader.FadeSprite(spriteRenderer, _targetColor.Value, _duration.Value, Vector2.zero, delegate {
				if (waitUntilFinished)
				{
					cameraController.waiting = false;
					Continue();
				}
			});

			if (!waitUntilFinished)
			{
				Continue();
			}
		}

		public override string GetSummary()
		{
			if (spriteRenderer == null)
			{
				return "Error: No sprite renderer selected";
			}

			return spriteRenderer.name + " to " + _targetColor.Value.ToString();
		}

		public override Color GetButtonColor()
		{
			return new Color32(221, 184, 169, 255);
		}

		#region Backwards compatibility

		[HideInInspector] [FormerlySerializedAs("duration")] public float durationOLD;
		[HideInInspector] [FormerlySerializedAs("targetColor")] public Color targetColorOLD;

		public virtual void OnBeforeSerialize()
		{}

		public virtual void OnAfterDeserialize()
		{
			if (durationOLD != default(float))
			{
				_duration.Value = durationOLD;
				durationOLD = default(float);
			}
			if (targetColorOLD != default(Color))
			{
				_targetColor.Value = targetColorOLD;
				targetColorOLD = default(Color);
			}
		}

		#endregion
	}

}
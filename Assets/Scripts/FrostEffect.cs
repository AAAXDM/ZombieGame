using UnityEngine;
using System.Collections;

namespace Download
{
    [ExecuteInEditMode]
    [AddComponentMenu("Image Effects/Frost")]
    public class FrostEffect : MonoBehaviour
    {
        public float FrostAmount; //0-1 (0=minimum Frost, 1=maximum frost)
        public float EdgeSharpness = 1; //>=1
        public float minFrost = 0; //0-1
        public float maxFrost = 1; //0-1
        public float seethroughness = 0.2f; //blends between 2 ways of applying the frost effect: 0=normal blend mode, 1="overlay" blend mode
        public float distortion = 0.1f; //how much the original image is distorted through the frost (value depends on normal map)
        public Texture2D Frost; //RGBA
        public Texture2D FrostNormals; //normalmap
        public Shader Shader; //ImageBlendEffect.shader
        private bool isActive = false;

        private Material material;

        public bool IsActive => isActive;
        private void Awake()
        {
            material = new Material(Shader);
            material.SetTexture("_BlendTex", Frost);
            material.SetTexture("_BumpMap", FrostNormals);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (!Application.isPlaying)
            {
                material.SetTexture("_BlendTex", Frost);
                material.SetTexture("_BumpMap", FrostNormals);
                EdgeSharpness = Mathf.Max(1, EdgeSharpness);
            }
            material.SetFloat("_BlendAmount", Mathf.Clamp01(Mathf.Clamp01(FrostAmount) * (maxFrost - minFrost) + minFrost));
            material.SetFloat("_EdgeSharpness", EdgeSharpness);
            material.SetFloat("_SeeThroughness", seethroughness);
            material.SetFloat("_Distortion", distortion);

            Graphics.Blit(source, destination, material);
        }

        public void ChangeFrostAmount(float startValue, float endValue)
        {
            isActive = true;
            StartCoroutine(IncreaseChangeFrostAmountRoutine(startValue, endValue));
        }

        private IEnumerator IncreaseChangeFrostAmountRoutine(float startValue, float endValue)
        {
            float iterationTime = 0.05f;
            float iterationValue = 0.02f;
            FrostAmount = startValue;
            while (FrostAmount < endValue)
            {
                FrostAmount += iterationValue;
                yield return new WaitForSeconds(iterationTime);
            }
            StartCoroutine(DecreaseChangeFrostAmountRoutine(startValue));
        }

        private IEnumerator DecreaseChangeFrostAmountRoutine(float startValue)
        {
            float iterationTime = 0.05f;
            float iterationValue = 0.01f;
            while (FrostAmount > startValue)
            {
                FrostAmount -= iterationValue;
                yield return new WaitForSeconds(iterationTime);
            }
            isActive = false;
        }
    }
}
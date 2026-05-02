using UnityEngine;
using UnityEngine.UI;

namespace LiverTransplantAR.Scenarios
{
    public class ARVisualsController : MonoBehaviour
    {
        [Header("Scanline Settings")]
        public Transform ScanlineQuad;
        public float ScanSpeed = 1.5f;
        public float ScanRange = 0.25f;

        [Header("Idle Animation")]
        public Transform RotationTarget;
        public float BreathSpeed = 1.2f;
        public float BreathAmount = 0.05f;
        public bool EnableAutoRotation = true;
        public float RotationSpeed = 10f;

        [Header("Floating HUD Settings")]
        public Transform HUDPanel;
        public float FloatSpeed = 0.8f;
        public float FloatAmount = 0.02f;

        [Header("Pulse Settings")]
        public Renderer LiverRenderer;
        public LiverTransplantAR.Data.SimulationState Data;
        public Color PulseColor = Color.cyan;
        
        private MaterialPropertyBlock _propBlock;
        private float _pulseTimer = 0f;
        private bool _isPulsing = false;
        private float _currentSteatosis = 0f;
        
        private Vector3 _initialScale;
        private Vector3 _initialPosition;
        private Vector3 _initialHUDPosition;

        void Start()
        {
            _propBlock = new MaterialPropertyBlock();
            if (LiverRenderer != null)
            {
                _initialScale = LiverRenderer.transform.localScale;
                _initialPosition = LiverRenderer.transform.localPosition;
                
                var animator = LiverRenderer.GetComponentInParent<Animator>();
                if (animator != null) animator.enabled = false;
            }
            if (HUDPanel != null) _initialHUDPosition = HUDPanel.localPosition;
        }

        void Update()
        {
            AnimateScanline();
            AnimateHUD();
            AnimateLiverIdle();
            UpdateShaderProperties();
        }

        private void AnimateScanline()
        {
            if (ScanlineQuad == null) return;
            // SLOWER SCAN: 0.5f speed
            float y = Mathf.PingPong(Time.time * 0.5f, ScanRange * 2) - ScanRange;
            ScanlineQuad.localPosition = new Vector3(0, y, 0);
        }

        private void AnimateLiverIdle()
        {
            if (LiverRenderer == null) return;
            
            // SLOWER BREATH
            float breath = 1.0f + Mathf.Sin(Time.time * 0.6f) * (BreathAmount * 0.5f);
            LiverRenderer.transform.localScale = _initialScale * breath;

            float floatY = Mathf.Sin(Time.time * 0.3f) * 0.03f; 
            LiverRenderer.transform.localPosition = _initialPosition + new Vector3(0, floatY, 0);

            if (EnableAutoRotation)
            {
                Transform target = RotationTarget != null ? RotationTarget : LiverRenderer.transform;
                target.Rotate(Vector3.up, RotationSpeed * Time.deltaTime, Space.World);
            }
        }

        private void AnimateHUD()
        {
            if (HUDPanel == null) return;
            // VISIBLE FLOATING: Now it moves 8cm up/down with a smooth medical rhythm
            float offset = Mathf.Sin(Time.time * 1.0f) * 0.08f; 
            HUDPanel.localPosition = _initialHUDPosition + new Vector3(0, offset, 0);
        }

        private void UpdateShaderProperties()
        {
            if (LiverRenderer == null) return;
            
            // 1. Handle Pulse Timer
            if (_isPulsing)
            {
                _pulseTimer -= Time.deltaTime * 1.5f;
                if (_pulseTimer <= 0) { _pulseTimer = 0; _isPulsing = false; }
            }

            // 2. Handle Steatosis (Fatty Liver) Visuals
            if (Data != null)
            {
                float targetSteatosis = Data.IsFattyDiet ? 1.0f : 0.0f;
                _currentSteatosis = Mathf.Lerp(_currentSteatosis, targetSteatosis, Time.deltaTime * 0.8f);
            }

            // 3. Apply to Renderer
            LiverRenderer.GetPropertyBlock(_propBlock);
            
            // Growth Offset from Simulation Data
            if (Data != null)
            {
                // Map 0.3-1.0 growth to 0.0-0.5 shader offset
                float growthOffset = Mathf.Clamp((Data.GrowthPercentage - 0.3f) * 0.7f, 0, 0.5f);
                _propBlock.SetFloat("_GrowthOffset", growthOffset);
            }

            _propBlock.SetFloat("_PulseAmount", _pulseTimer);
            _propBlock.SetColor("_PulseColor", PulseColor);
            _propBlock.SetFloat("_SteatosisAmount", _currentSteatosis);
            LiverRenderer.SetPropertyBlock(_propBlock);
        }

        public void TriggerPulse(Color color)
        {
            Debug.Log("<color=cyan>AR Visuals:</color> Triggering pulse effect.");
            PulseColor = color;
            _pulseTimer = 1.0f;
            _isPulsing = true;
        }
    }
}

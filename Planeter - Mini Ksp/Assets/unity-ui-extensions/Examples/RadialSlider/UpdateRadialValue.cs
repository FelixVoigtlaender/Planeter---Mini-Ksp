namespace UnityEngine.UI.Extensions.Examples
{
    public class UpdateRadialValue : MonoBehaviour
    {
        public InputField input;
        //public RadialSlider slider;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        public void UpdateSliderValue()
        {
            float value;
            float.TryParse(input.text, out value);
            //slider.Value = value;

            Debug.LogError("Removed due to Modern UI Pack");
        }

        public void UpdateSliderAndle()
        {
            int value;
            int.TryParse(input.text, out value);
            //slider.Angle = value;
            
            Debug.LogError("Removed due to Modern UI Pack");
        }
    }
}
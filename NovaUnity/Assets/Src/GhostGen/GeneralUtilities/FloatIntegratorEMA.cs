namespace GhostGen
{
    public struct FloatIntegratorEMA
    {
        private float _ratio;
        private bool _isFirst;

        public float average  { get; private set; }
        
        public bool isInitialized
        {
            get { return _ratio != 0.0f; }
        }

        public FloatIntegratorEMA(int count)
        {
            average = 0.0f;
            _ratio   = 2.0f / (count + 1);
            
            _isFirst = true;
        }

        public void Integrate(float value)
        {
            if(_isFirst)
            {
                average  = value;
                _isFirst = false;
            }
            else
            {
                average += _ratio * (value - average);
            }
        }    
    }
}

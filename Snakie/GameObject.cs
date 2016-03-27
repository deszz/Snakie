using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snakie
{
    public abstract class GameObject : AppObject
    {
        private bool isEnabled = true;
        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                isEnabled = value;
                if (isEnabled)
                    OnEnabled();
                else
                    OnDisabled();
            }
        }

        public bool IsDestroyed
        { get; private set; }

        public void Destroy(bool force = false)
        {
            bool canceled = false;
            OnDestroy(ref canceled);

            IsDestroyed = !canceled || force;
        }

        protected virtual void OnEnabled() { }
        protected virtual void OnDisabled() { }
    }
}

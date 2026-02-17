using System;
using System.Collections.Generic;

namespace Abim.Enterprise.Core.Testing.Setup.DataBuilders
{
    public abstract class ResourceDataBuilder<T, TBuilder>
    {
        #region private variables

        private T domanData;
        private Func<T> resourceDataCreator;
        private bool needReset = false;

        #endregion

        #region contractors

        protected ResourceDataBuilder(T data)
        {
            domanData = data;
        }

        protected ResourceDataBuilder(Func<T> resourceDataCreator)
        {
            this.resourceDataCreator = resourceDataCreator;
            domanData = resourceDataCreator();
        }

        #endregion

        #region Public Methods

        public ResourceDataBuilder<T, TBuilder> With(Action<T> action)
        {
            if (needReset)
            {
                domanData = resourceDataCreator();
                needReset = false;
            }
                
            action(domanData);
            return this;
        }

        public ResourceDataBuilder<T, TBuilder> WithList(List<Action<T>> actions)
        {
            if (actions != null)
            {
                foreach (var a in actions)
                    With(a);
            }

            return this;
        }

        public T Build()
        {
            needReset = true;
            return domanData;
        }

        #endregion
    }
}

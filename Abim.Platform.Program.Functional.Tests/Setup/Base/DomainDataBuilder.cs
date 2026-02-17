using System;
using System.Collections.Generic;

namespace Abim.Enterprise.Core.Testing.Setup.Builders
{
    public abstract class DomainDataBuilder<T, TBuilder>
    {
        #region private variables

        private T domanData;
        private Func<T> domainDataCreator;
        private bool needReset = false;

        #endregion

        #region contractors

        protected DomainDataBuilder(T data)
        {
            domanData = data;
        }

        protected DomainDataBuilder(Func<T> domainDataCreator)
        {
            this.domainDataCreator = domainDataCreator;
            domanData = domainDataCreator();
        }

        #endregion

        #region Public Methods

        public DomainDataBuilder<T, TBuilder> With(Action<T> action)
        {
            if (needReset)
            {
                domanData = domainDataCreator();
                needReset = false;
            }

            action(domanData);
            return this;
        }

        public DomainDataBuilder<T, TBuilder> WithList(List<Action<T>> actions)
        {
            if (actions != null)
            { 
                foreach(var a in actions)
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

using System;
using Castle.DynamicProxy;

namespace ActoR
{
    public static class ActorFactory
    {
        public static TInterface Create<TInterface, TConcrete>(Func<TConcrete> instanceFactory, ActorAffinity affinity)
            where TConcrete : TInterface
            where TInterface : class
        {
            var generator = new ProxyGenerator();

            TInterface proxy = generator.CreateInterfaceProxyWithoutTarget<TInterface>(
                new ActorInterceptor<TConcrete>(instanceFactory(), affinity));

            return proxy;

        }

        public static Actor Create(ActorAffinity affinity)
        {
            return new Actor(affinity);
        }


    }
}

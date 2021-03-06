﻿using System;
using System.Threading.Tasks;
using Xunit;
using ActoR;

namespace ActoRTest
{

    public class ExceptionTestsOnTypedActorMethodInv
    {

        [Theory]
        [InlineData(ActorAffinity.LongRunningThread)]
        [InlineData(ActorAffinity.ThreadPoolThread)]
        public async void TestTask(ActorAffinity affinity)
        {
            ITypedThrowing actor = ActorFactory.Create<ITypedThrowing, TypedThrowing>(() => new TypedThrowing(), affinity);
            await Assert.ThrowsAsync<TypedThrowing.AException>(async () => await actor.A());

        }

        [Theory]
        [InlineData(ActorAffinity.LongRunningThread)]
        [InlineData(ActorAffinity.ThreadPoolThread)]
        public async void TestTaskOfT(ActorAffinity affinity)
        {
            ITypedThrowing actor = ActorFactory.Create<ITypedThrowing, TypedThrowing>(() => new TypedThrowing(), affinity);
            await Assert.ThrowsAsync<TypedThrowing.BException>(async () => await actor.B());
        }

        public interface ITypedThrowing
        {
            Task<bool> A();
            Task B();
        }

        class TypedThrowing : ITypedThrowing
        {
            internal class AException : Exception {}
            public Task<bool> A()
            {
                throw new AException();
            }

            internal class BException : Exception {}
            public Task B()
            {
                throw new BException();
            }
        }
    }
}

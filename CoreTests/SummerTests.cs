using System;
using Core.SummerCore;
using NUnit.Framework;
using LifeCycle = Core.SummerCore.LifeCycle;

namespace CoreTests
{
    [TestFixture]
    public class SummerTests
    {
        interface A
        {
            int rnd { get; }
        }

        interface B
        {
            int rnd { get; }
            A a { get; }
        }

        interface C<T>
        {
            int rnd { get; }
            T t { get; }
        }

        class CI1 : C<A>
        {
            private int _rnd = new Random().Next();
            public int rnd => _rnd;
            private A _a;
            public A t => _a;
        }
        
        class CI2 : C<A>
        {
            private int _rnd = new Random().Next();
            public int rnd => _rnd;
            private A _a;
            public A t => _a;
        }
        
        class AI : A
        {
            private int _rnd = new Random().Next();
            public int rnd => _rnd;
        }

        class BI : B
        {
            private A _a;
            public A a => _a;
            private int _rnd = new Random().Next();
            public int rnd => _rnd;

            public BI(A a)
            {
                _a = a;
            }
        }

        [Test]
        public void Resolve_DifferentValues_Instance()
        {
            var c = new SummerConfig();
            c.Register<A, AI>(LifeCycle.Instance);
            var s = new Summer(c);
            int prev = s.Resolve<A>().rnd;
            Assert.Multiple(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    int curr = s.Resolve<A>().rnd;
                    Assert.AreNotEqual(prev, curr);
                }
            });
        }
        
        [Test]
        public void Resolve_SameValues_Singleton()
        {
            var c = new SummerConfig();
            c.Register<A, AI>(LifeCycle.Singleton);
            var s = new Summer(c);
            int prev = s.Resolve<A>().rnd;
            Assert.Multiple(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    int curr = s.Resolve<A>().rnd;
                    Assert.AreEqual(prev, curr);
                }
            });
        }

        [Test]
        public void Resolve_Instance_Instance()
        {
            var c = new SummerConfig();
            c.Register<A, AI>(LifeCycle.Instance);
            c.Register<B, BI>(LifeCycle.Instance);
            var s = new Summer(c);
            B prev = s.Resolve<B>();
            Assert.Multiple(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    B curr = s.Resolve<B>();
                    Assert.AreNotEqual(prev.rnd, curr.rnd);
                    Assert.AreNotEqual(prev.a.rnd, curr.a.rnd);
                }
            });
        }
        
        [Test]
        public void Resolve_Instance_Singleton()
        {
            var c = new SummerConfig();
            c.Register<A, AI>(LifeCycle.Instance);
            c.Register<B, BI>(LifeCycle.Singleton);
            var s = new Summer(c);
            int preva = s.Resolve<A>().rnd;
            B prev = s.Resolve<B>();
            Assert.Multiple(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    int curra = s.Resolve<A>().rnd;
                    B curr = s.Resolve<B>();
                    Assert.AreNotEqual(preva, curra);
                    Assert.AreEqual(prev.rnd, curr.rnd);
                    Assert.AreEqual(prev.a.rnd, curr.a.rnd);
                }
            });
        }
        
        [Test]
        public void Resolve_Singleton_Instance()
        {
            var c = new SummerConfig();
            c.Register<A, AI>(LifeCycle.Singleton);
            c.Register<B, BI>(LifeCycle.Instance);
            var s = new Summer(c);
            int preva = s.Resolve<A>().rnd;
            B prev = s.Resolve<B>();
            Assert.Multiple(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    int curra = s.Resolve<A>().rnd;
                    B curr = s.Resolve<B>();
                    Assert.AreEqual(preva, curra);
                    Assert.AreNotEqual(prev.rnd, curr.rnd);
                    Assert.AreEqual(prev.a.rnd, curr.a.rnd);
                }
            });
        }
        
        [Test]
        public void Resolve_Singleton_Singleton()
        {
            var c = new SummerConfig();
            c.Register<A, AI>(LifeCycle.Singleton);
            c.Register<B, BI>(LifeCycle.Singleton);
            var s = new Summer(c);
            int preva = s.Resolve<A>().rnd;
            B prev = s.Resolve<B>();
            Assert.Multiple(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    int curra = s.Resolve<A>().rnd;
                    B curr = s.Resolve<B>();
                    Assert.AreEqual(preva, curra);
                    Assert.AreEqual(prev.rnd, curr.rnd);
                    Assert.AreEqual(prev.a.rnd, curr.a.rnd);
                }
            });
        }

        [Test]
        public void ResolveAll_Count_Correct()
        {
            var c = new SummerConfig();
            c.Register<C<A>, CI1>(LifeCycle.Instance);
            c.Register<C<A>, CI2>(LifeCycle.Instance);
            var s = new Summer(c);
            Assert.AreEqual(2, s.ResolveAll<C<A>>().Length);
        }
    }
}
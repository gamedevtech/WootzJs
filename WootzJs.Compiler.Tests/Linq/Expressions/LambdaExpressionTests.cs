#region License
//-----------------------------------------------------------------------
// <copyright>
// The MIT License (MIT)
// 
// Copyright (c) 2014 Kirk S Woll
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
//-----------------------------------------------------------------------
#endregion

using System;
using System.Linq.Expressions;
using WootzJs.Testing;

namespace WootzJs.Compiler.Tests.Linq.Expressions
{
    [TestFixture]
    public class LambdaExpressionTests
    {
        [Test]
        public void Func()
        {
            Expression<Func<Func<int, int>>> lambda = () => x => x;
            var expression = (LambdaExpression)lambda.Body;

            expression.Parameters.Count.AssertEquals(1);
            expression.Body.AssertEquals(expression.Parameters[0]);
        }

        [Test]
        public void CompileConstant()
        {
            Expression<Func<int>> return5 = () => 5;
            var func = return5.Compile();
            Assert.AssertEquals(func(), 5);
        }

        [Test]
        public void CompileReturnParameter()
        {
            Expression<Func<int, int>> returnParameter = x => x;
            var func = returnParameter.Compile();
            Assert.AssertEquals(func(5), 5);
        }

        [Test]
        public void CompileAdder()
        {
            Expression<Func<int, int>> added = x => x + 1;
            var func = added.Compile();
            Assert.AssertEquals(func(5), 6);
        }
    }
}

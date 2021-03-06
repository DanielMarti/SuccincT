﻿using System;
using NUnit.Framework;
using SuccincT.Options;
using SuccincT.PatternMatchers;
using static NUnit.Framework.Assert;
using static SuccincT.Functional.Unit;

namespace SuccincTTests.SuccincT.Options
{
    [TestFixture]
    public sealed class ValueOrErrorTests
    {
        [Test]
        public void WhenValueIsSet_ValueSuppliedToFunction()
        {
            var valueOrError = ValueOrError.WithValue("1");
            var result = valueOrError.Match<string>().Value().Do(s => "v" + s).Error().Do(s => "e" + s).Result();
            AreEqual("v1", result);
        }

        [Test]
        public void WhenErrorIsSet_ErrorSuppliedToFunction()
        {
            var valueOrError = ValueOrError.WithError("2");
            var result = valueOrError.Match<string>().Value().Do(s => "v" + s).Error().Do(s => "e" + s).Result();
            AreEqual("e2", result);
        }

        [Test]
        public void WhenValueIsSet_HasValueIsTrue()
        {
            var valueOrError = ValueOrError.WithValue("1");
            IsTrue(valueOrError.HasValue);
        }

        [Test]
        public void WhenErrorIsSet_HasValueIsFalse()
        {
            var valueOrError = ValueOrError.WithError("2");
            IsFalse(valueOrError.HasValue);
        }

        [Test]
        public void WhenValueIsSet_ValueCanBeAccessed()
        {
            var valueOrError = ValueOrError.WithValue("1");
            AreEqual("1", valueOrError.Value);
        }

        [Test]
        public void WhenErrorIsSet_ErrorCanBeAccessed()
        {
            var valueOrError = ValueOrError.WithError("2");
            AreEqual("2", valueOrError.Error);
        }

        [Test]
        public void WhenValueIsSet_AccessingErrorCausesException()
        {
            var valueOrError = ValueOrError.WithValue("2");
            Throws<InvalidOperationException>(() => Ignore(valueOrError.Error));
        }

        [Test]
        public void WhenErrorIsSet_AccessingValueCausesAnException()
        {
            var valueOrError = ValueOrError.WithError("2");
            Throws<InvalidOperationException>(() => Ignore(valueOrError.Value));
        }

        [Test]
        public void WhenValueIsSet_PrintStringYieldsValue()
        {
            var valueOrError = ValueOrError.WithValue("42");
            AreEqual("Value of 42", valueOrError.ToString());
        }

        [Test]
        public void WhenErrorIsSet_PrintStringYieldsError()
        {
            var valueOrError = ValueOrError.WithError("42");
            AreEqual("Error of 42", valueOrError.ToString());
        }

        [Test]
        public void WhenErrorIsSetAndNoErrorMatch_ElseResultIsReturned()
        {
            var valueOrError = ValueOrError.WithError("2");
            var result = valueOrError.Match<int>().Value().Do(x => 0).Else(x => 3).Result();
            AreEqual(3, result);
        }

        [Test]
        public void WhenValueIsSetAndNoErrorMatch_ElseResultIsReturned()
        {
            var valueOrError = ValueOrError.WithValue("1");
            var result = valueOrError.Match<int>().Error().Do(x => 2).Else(x => 3).Result();
            AreEqual(3, result);
        }

        [Test]
        public void WhenValueIsSetAndNoValueMatchDefined_ExceptionThrown()
        {
            var valueOrError = ValueOrError.WithValue("1");
            Throws<NoMatchException>(() => valueOrError.Match<int>().Error().Do(x => 2).Result());
        }

        [Test]
        public void WhenErrorIsSetAndNoErrorMatchDefined_ExceptionThrown()
        {
            var valueOrError = ValueOrError.WithError("1");
            Throws<NoMatchException>(() => Ignore(valueOrError.Match<int>().Value().Do(x => 2).Result()));
        }

        [Test]
        public void CreatingWithNullValue_CausesNullException()
        {
            Throws<ArgumentNullException>(() => ValueOrError.WithValue(null));
        }

        [Test]
        public void CreatingWithNullError_CausesNullException()
        {
            Throws<ArgumentNullException>(() => ValueOrError.WithError(null));
        }

        [Test]
        public void WhenValue_SimpleValueDoWithExpressionSupported()
        {
            var valueOrError = ValueOrError.WithValue("1");
            var result = valueOrError.Match<int>().Value().Do(1).Error().Do(2).Result();
            AreEqual(1, result);
        }

        [Test]
        public void WhenSome_SomeOfDoWithExpressionSupported()
        {
            var valueOrError = ValueOrError.WithValue("1");
            var result = valueOrError.Match<int>().Value().Of("1").Do(1).Value().Do(2).Error().Do(3).Result();
            AreEqual(1, result);
        }

        [Test]
        public void WhenSome_SomeWhereDoWithExpressionSupported()
        {
            var valueOrError = ValueOrError.WithValue("1");
            var result = valueOrError.Match<int>()
                                     .Value().Where(x => x == "1").Do(0).Value().Do(2).Error().Do(3).Result();
            AreEqual(0, result);
        }

        [Test]
        public void WhenError_SimpleErrorDoWithExpressionSupported()
        {
            var valueOrError = ValueOrError.WithError("1");
            var result = valueOrError.Match<int>().Value().Do(1).Error().Do(2).Result();
            AreEqual(2, result);
        }

        [Test]
        public void WhenError_ErrorOfDoWithExpressionSupported()
        {
            var valueOrError = ValueOrError.WithError("1");
            var result = valueOrError.Match<int>().Value().Of("1").Do(1)
                                     .Value().Do(2)
                                     .Error().Of("1").Do(3).Result();
            AreEqual(3, result);
        }

        [Test]
        public void WhenError_ErrorWhereDoWithExpressionSupported()
        {
            var valueOrError = ValueOrError.WithError("1");
            var result = valueOrError.Match<int>()
                                     .Value().Where(x => x == "1").Do(0)
                                     .Value().Do(2)
                                     .Error().Where(x => x == "1").Do(3).Result();
            AreEqual(3, result);
        }
    }
}
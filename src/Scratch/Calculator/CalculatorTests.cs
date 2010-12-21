//  * **********************************************************************************
//  * Copyright (c) Clinton Sheppard
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **********************************************************************************

using System.Collections.Generic;

using FluentAssert;

using NUnit.Framework;

using Rhino.Mocks;

using StructureMap.AutoMocking;

namespace Scratch.Calculator
{
    /// <summary>
    /// http://simpleprogrammer.com/2010/12/12/back-to-basics-why-unit-testing-is-hard/
    /// </summary>
    public class CalculatorTests
    {
        [TestFixture]
        public class When_asked_to_Add
        {
            private Calculator _calculator;
            private int _firstInput;
            private int _result;
            private int _secondInput;
            private IStorageService _storageService;

            [SetUp]
            public void BeforeEachTest()
            {
                _firstInput = 0;
                _secondInput = 0;
                var mocker = new RhinoAutoMocker<Calculator>();
                _calculator = mocker.ClassUnderTest;
                _storageService = mocker.Get<IStorageService>();
            }

            [TearDown]
            public void AfterEachTest()
            {
                _storageService.VerifyAllExpectations();
            }

            [Test]
            public void Given_values_3_and_4()
            {
                Test.Verify(
                    with_3,
                    and_4,
                    when_asked_to_Add_them,
                    should_call_the_storage_service_to_store_the_sum,
                    should_return_the_sum_of_the_inputs
                    );
            }

            private void and_4()
            {
                _secondInput = 4;
            }

            private void should_call_the_storage_service_to_store_the_sum()
            {
                _storageService.AssertWasCalled(x => x.Store(_firstInput + _secondInput));
            }

            private void should_return_the_sum_of_the_inputs()
            {
                _result.ShouldBeEqualTo(_firstInput + _secondInput);
            }

            private void when_asked_to_Add_them()
            {
                _result = _calculator.Add(_firstInput, _secondInput);
            }

            private void with_3()
            {
                _firstInput = 3;
            }
        }

        [TestFixture]
        public class When_asked_to_GetHistory
        {
            private List<int> _addResult;
            private Calculator _calculator;
            private IList<int> _result;
            private IStorageService _storageService;

            [SetUp]
            public void BeforeEachTest()
            {
                _addResult = new List<int>();
                var mocker = new RhinoAutoMocker<Calculator>();
                _calculator = mocker.ClassUnderTest;
                _storageService = mocker.Get<IStorageService>();
            }

            [TearDown]
            public void AfterEachTest()
            {
                _storageService.VerifyAllExpectations();
            }

            [Test]
            public void Given_Add_has_been_called_3_times()
            {
                Test.Verify(
                    with_call_to_Add__1_3,
                    with_call_to_Add__2_5,
                    with_call_to_Add__3_6,
                    expect_call_to_get_history_from_storage_service,
                    when_asked_to_GetHistory,
                    should_not_return_null,
                    should_return_3_records,
                    first_record_should_contain_result_of_first_Add,
                    second_record_should_contain_result_of_second_Add,
                    third_record_should_contain_result_of_third_Add
                    );
            }

            private void VerifyStorageValueMatchesAddResult(int index)
            {
                _result[index].ShouldBeEqualTo(_addResult[index]);
            }

            private void expect_call_to_get_history_from_storage_service()
            {
                _storageService.Expect(x => x.IsServiceOnline()).Return(true);
                _storageService.Expect(x => x.GetHistorySession(1)).Return(new[] { 4, 7, 9 });
            }

            private void first_record_should_contain_result_of_first_Add()
            {
                VerifyStorageValueMatchesAddResult(0);
            }

            private void second_record_should_contain_result_of_second_Add()
            {
                VerifyStorageValueMatchesAddResult(1);
            }

            private void should_not_return_null()
            {
                _result.ShouldNotBeNull();
            }

            private void should_return_3_records()
            {
                _result.Count.ShouldBeEqualTo(3);
            }

            private void third_record_should_contain_result_of_third_Add()
            {
                VerifyStorageValueMatchesAddResult(2);
            }

            private void when_asked_to_GetHistory()
            {
                _result = _calculator.GetHistory();
            }

            private void with_call_to_Add__1_3()
            {
                _addResult.Add(_calculator.Add(1, 3));
            }

            private void with_call_to_Add__2_5()
            {
                _addResult.Add(_calculator.Add(2, 5));
            }

            private void with_call_to_Add__3_6()
            {
                _addResult.Add(_calculator.Add(3, 6));
            }
        }
    }
}
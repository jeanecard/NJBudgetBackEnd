using NJBudgetBackEnd.Models;
using NJBudgetWBackend.Business;
using System;
using System.Collections.Generic;
using Xunit;

namespace NJBudgetWBackEndTests
{
    public class BudgetProcessorTest
    {
        private BudgetProcessor bProcesor = new BudgetProcessor();
        [Fact]
        public void ProcessBudgetSpentAndLeft_With_Add_Operation_Expect_Left_Spent_Epargne_Updated()
        {
            Compte cpt = new Compte();
            List<Operation> ops = new List<Operation>();
            cpt.BudgetExpected = 100;
            Operation ope1 = new Operation() {Value = 5, DateOperation = new DateTime(2021, 1, 1) };
            Operation ope2 = new Operation() { Value = 5, DateOperation = new DateTime(2021, 1, 1) };

            ops.Add(ope1);
            ops.Add(ope2);

            bProcesor.ProcessBudgetSpentAndLeft(cpt, ops, 1, 2021);
            Assert.True(cpt.BudgetLeft == 90);
            Assert.True(cpt.BudgetProvision == 10);
            Assert.True(cpt.BudgetConsummed == 10);
        }

        [Fact]
        public void ProcessBudgetSpentAndLeft_With_Remove_Operation_Expect_Left_Spent_Epargne_Updated()
        {
            Compte cpt = new Compte();
            List<Operation> ops = new List<Operation>();
            cpt.BudgetExpected = 100;
            Operation ope1 = new Operation() { Value = -5, DateOperation = new DateTime(2021, 1, 1) };
            Operation ope2 = new Operation() { Value = -5, DateOperation = new DateTime(2021, 1, 1) };

            ops.Add(ope1);
            ops.Add(ope2);
            bProcesor.ProcessBudgetSpentAndLeft(cpt, ops, 1, 2021);
            Assert.True(cpt.BudgetLeft == 90);
            Assert.True(cpt.BudgetProvision == 0);
            Assert.True(cpt.BudgetConsummed == 10);
        }


        [Fact]
        public void ProcessBudgetSpentAndLeft_With_Add_And_Remove_Operation_Expect_Left_Spent_Epargne_Updated()
        {
            Compte cpt = new Compte();
            List<Operation> ops = new List<Operation>();
            cpt.BudgetExpected = 100;
            Operation ope1 = new Operation() { Value = -5, DateOperation = new DateTime(2021, 1, 1) };
            Operation ope2 = new Operation() { Value = 15, DateOperation = new DateTime(2021, 1, 1) };
            Operation ope3 = new Operation() { Value = -20, DateOperation = new DateTime(2021, 1, 1) };

            ops.Add(ope1);
            ops.Add(ope2);
            ops.Add(ope3);

            bProcesor.ProcessBudgetSpentAndLeft(cpt, ops, 1, 2021);
            Assert.True(cpt.BudgetLeft == 60);
            Assert.True(cpt.BudgetProvision == 15);
            Assert.True(cpt.BudgetConsummed == 40);
        }

        [Fact]
        public void ProcessBudgetSpentAndLeft_With_Epargne_Consomme_Expect_Left_Spent_Epargne_Updated()
        {
            Compte cpt = new Compte();
            List<Operation> ops = new List<Operation>();
            cpt.BudgetExpected = 100;
            Operation ope1 = new Operation() { Value = 100, DateOperation = new DateTime(2021, 1, 1) };
            Operation ope2 = new Operation() { Value = -120, DateOperation = new DateTime(2021, 1, 1) };

            ops.Add(ope1);
            ops.Add(ope2);

            bProcesor.ProcessBudgetSpentAndLeft(cpt, ops, 1, 2021);
            Assert.True(cpt.BudgetLeft == -120);
            Assert.True(cpt.BudgetProvision == 100);
            Assert.True(cpt.BudgetConsummed == 220);

            //Suppression de l'épargne
            ops.RemoveAt(0);
            bProcesor.ProcessBudgetSpentAndLeft(cpt, ops, 1, 2021);
            Assert.True(cpt.BudgetLeft == -20);
            Assert.True(cpt.BudgetProvision == 0);
            Assert.True(cpt.BudgetConsummed == 120);
        }

        [Fact]
        public void ProcessBudgetSpentAndLeft_Epargne_Et_Depense_Expected_Budget_Expect_No_Left()
        {
            Compte cpt = new Compte();
            List<Operation> ops = new List<Operation>();
            cpt.BudgetExpected = 100;
            Operation ope1 = new Operation() { Value = 50, DateOperation = new DateTime(2021, 1, 1) };
            Operation ope2 = new Operation() { Value = -50, DateOperation = new DateTime(2021, 1, 1) };

            ops.Add(ope1);
            ops.Add(ope2);

            bProcesor.ProcessBudgetSpentAndLeft(cpt, ops, 1, 2021);
            Assert.True(cpt.BudgetLeft == 0);
            Assert.True(cpt.BudgetProvision == 50);
            Assert.True(cpt.BudgetConsummed == 100);
        }


        [Fact]
        public void ProcessBudgetSpentAndLeft_Depense_Exeed_Budget_Expect_Left_Neg()
        {
            Compte cpt = new Compte();
            List<Operation> ops = new List<Operation>();
            cpt.BudgetExpected = 100;
            Operation ope1 = new Operation() { Value = 50, DateOperation = new DateTime(2021, 1, 1) };
            Operation ope2 = new Operation() { Value = -60, DateOperation = new DateTime(2021, 1, 1) };

            ops.Add(ope1);
            ops.Add(ope2);

            bProcesor.ProcessBudgetSpentAndLeft(cpt, ops, 1, 2021);
            Assert.True(cpt.BudgetLeft == -10);
            Assert.True(cpt.BudgetProvision == 50);
            Assert.True(cpt.BudgetConsummed == 110);
        }

       [Fact]
       public void ProcessStateDeleteOnly_LessExpense_ThanExpectedBudget_Expect_Good()
        {
            Compte cpt = new Compte();
            cpt.BudgetExpected = 300;
            cpt.OperationAllowed = OperationTypeEnum.DeleteOnly;
            bProcesor.ProcessStateDeleteOnly(cpt, -1000, 5, 1);
            Assert.True(cpt.State == CompteStatusEnum.Good);

            bProcesor.ProcessStateDeleteOnly(cpt, -299, 1, 1);
            Assert.True(cpt.State == CompteStatusEnum.Good);
        }

        [Fact]
        public void ProcessStateDeleteOnly_MoreExpense_ThanExpected_ButLess_Than_15percent_Budget_Expect_Good()
        {
            Compte cpt = new Compte();
            cpt.BudgetExpected = 1000;
            cpt.OperationAllowed = OperationTypeEnum.DeleteOnly;
            bProcesor.ProcessStateDeleteOnly(cpt, -1149, 5, 1);
            Assert.True(cpt.State == CompteStatusEnum.Good);
        }

        [Fact]
        public void ProcessStateDeleteOnly_MoreExpense_ThanExpected_ButLess_Than_50percent_Budget_Expect_Good_Or_Warning()
        {
            Compte cpt = new Compte();
            cpt.BudgetExpected = 1000;
            cpt.OperationAllowed = OperationTypeEnum.DeleteOnly;
            bProcesor.ProcessStateDeleteOnly(cpt, -5499, 5, 12);
            Assert.True(cpt.State == CompteStatusEnum.Good);
            bProcesor.ProcessStateDeleteOnly(cpt, -5499, 5, 16);
            Assert.True(cpt.State == CompteStatusEnum.Warning);
        }

        [Fact]
        public void ProcessStateDeleteOnly_MoreExpense_ThanExpected_ButMore_Than_50percent_Budget_And_Less_Than_Expected_Expect_Warning()
        {
            Compte cpt = new Compte();
            cpt.BudgetExpected = 1000;
            cpt.OperationAllowed = OperationTypeEnum.DeleteOnly;
            bProcesor.ProcessStateDeleteOnly(cpt, -5501, 5, 12);
            Assert.True(cpt.State == CompteStatusEnum.Warning);
            bProcesor.ProcessStateDeleteOnly(cpt, -5501, 5, 16);
            Assert.True(cpt.State == CompteStatusEnum.Warning);
        }

        [Fact]
        public void ProcessStateDeleteOnly_MoreExpense_ThanBudgetExpected_Expect_Danger()
        {
            Compte cpt = new Compte();
            cpt.BudgetExpected = 1000;
            cpt.OperationAllowed = OperationTypeEnum.DeleteOnly;
            bProcesor.ProcessStateDeleteOnly(cpt, -6001, 5, 12);
            Assert.True(cpt.State == CompteStatusEnum.Danger);
            bProcesor.ProcessStateDeleteOnly(cpt, -6001, 5, 16);
            Assert.True(cpt.State == CompteStatusEnum.Danger);
        }

        [Fact]
        public void ProcessStateAddAndDelete_BudgetExpected_Not_Reached_But_Provision_LT_Depense_Expect_Warning()
        {
            Compte cpt = new Compte();
            cpt.BudgetExpected = 1000;
            cpt.OperationAllowed = OperationTypeEnum.AddAndDelete;
            bProcesor.ProcessStateAddAndDelete(cpt, 400, 300, 2);
            Assert.True(cpt.State == CompteStatusEnum.Warning);
        }

        [Fact]
        public void ProcessStateAddAndDelete_BudgetExpected_Not_Reached_But_Provision_GT_Depense_And_More_Than_75Percent_Expect_Good()
        {
            Compte cpt = new Compte();
            cpt.BudgetExpected = 500;
            cpt.OperationAllowed = OperationTypeEnum.AddAndDelete;
            bProcesor.ProcessStateAddAndDelete(cpt, 10, 800, 2);
            Assert.True(cpt.State == CompteStatusEnum.Good);
        }

        [Fact]
        public void ProcessStateAddAndDelete_BudgetExpected_Not_Reached_But_Provision_GT_Depense_And_At_30Percent_Expect_Good()
        {
            Compte cpt = new Compte();
            cpt.BudgetExpected = 500;
            cpt.OperationAllowed = OperationTypeEnum.AddAndDelete;
            bProcesor.ProcessStateAddAndDelete(cpt, 10, 300, 2);
            Assert.True(cpt.State == CompteStatusEnum.Warning);
        }

        [Fact]
        public void ProcessStateAddAndDelete_BudgetExpected_Not_Reached_But_Provision_GT_Depense_And_Less_25Percent_Expect_Good()
        {
            Compte cpt = new Compte();
            cpt.BudgetExpected = 500;
            cpt.OperationAllowed = OperationTypeEnum.AddAndDelete;
            bProcesor.ProcessStateAddAndDelete(cpt, 10, 100, 2);
            Assert.True(cpt.State == CompteStatusEnum.Danger);
        }
    }
}

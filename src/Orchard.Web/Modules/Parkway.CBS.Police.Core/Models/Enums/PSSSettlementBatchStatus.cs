namespace Parkway.CBS.Police.Core.Models.Enums
{
    public enum PSSSettlementBatchStatus
    {
        ReadyForQueueing = 0,
        PreQueue = 1,
        BatchPairedWithMatchingTransactions = 2,
        MaxSettlementPercentageCalculated = 3,
        MatchingCommandsToTransactions = 4,
        FinalProcessing = 5,
        MatchingFeePartiesWithRequestCommands = 6,
        SettingFallFlagForFeePartyCommandCombination = 7,
        SettingSplitItemCount = 8,
        PerformPercentageSplitForNonFallFlag = 9,
        PerformPercentageSplitForFallFlag = 10,
        AddingNonAdditionalSplitFeePartiesToBatchAggregate = 11,
        CheckingIfFeePartyInBatchHasAdditionalSplits = 12,
        MoveComputedItemsToBatchItemsTable = 13,
        MarkTransactionsAsSettled = 14,
        TransactionsMarkedAsSettled = 15,
        MatchingCommandFeePartiesWithTransactionsAndCommands = 16,
        AddingAdditionalSplitFeePartiesToBatchAggregate = 17,
        AddingAdditionalSplitFeePartiesToPercentageRecalculationBatchAggregate = 18,
        SettingFallFlagForPercentageRecalculationFeePartyBatchAggregate = 19,
        ComputeCommandPercentageForPercentageRecalculationNonFallFlags = 20,
        ComputeCommandPercentageForPercentageRecalculationFallFlags = 21,
        MoveComputedItemsWithNoAdditionalSplitsToBatchItemsTable = 22,
        MoveComputedItemsWithAdditionalSplitsToBatchItemsTable = 23,
        AddingAdditionalSplitFeePartiesForStateToPercentageRecalculationBatchAggregate = 24,
        AddingAdditionalSplitFeePartiesForZonalToPercentageRecalculationBatchAggregate = 25,
        MoveComputedItemsWithAdditionalSplitsForStateToBatchItemsTable = 26,
        MoveComputedItemsWithAdditionalSplitsForZonalToBatchItemsTable = 27,
    }
}
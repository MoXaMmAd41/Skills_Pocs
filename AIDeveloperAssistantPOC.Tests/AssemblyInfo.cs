// Console input/output is process-wide state that several tests redirect, so run tests one at a time.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

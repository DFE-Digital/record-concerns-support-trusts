namespace ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions
{
    // reference table, not required by app in normal use, but used to give context to the data
    public class DecisionTypeId
    {
        public Enums.Concerns.DecisionType Id { get; private set; }
        public string Name { get; private set; }

        private DecisionTypeId()
        {
                
        }

        public DecisionTypeId(Enums.Concerns.DecisionType decisionType, string name) : this()
        {
            Id = decisionType;
            Name = name;
        }
    }
}
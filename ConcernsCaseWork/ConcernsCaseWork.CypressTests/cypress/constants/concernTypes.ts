export type SfsoConcernType =
    | 'Actual and/or projected deficit'
    | 'Actual and/or projected cash shortfall'
    | 'Trust Closure'
    | 'Financial governance'
    | 'Financial management/ATH compliance'
    | 'Late financial returns'
    | 'Irregularity and/or self-reported fraud'
    | 'Force majeure';

export type RegionsGroupConcernType = 'Governance capability' | 'Non-compliance' | 'Safeguarding non-compliance';

export type ConcernType = SfsoConcernType | RegionsGroupConcernType;



// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Concerns.Data.Models
{
    public partial class SipEducationalperformancedata
    {
        public Guid Id { get; set; }
        public string SipName { get; set; }
        public decimal? SipMeetingexpectedstandardinrwm { get; set; }
        public decimal? SipMeetingexpectedstandardinrwmdisadv { get; set; }
        public decimal? SipMeetinghigherstandardinrwm { get; set; }
        public decimal? SipMeetinghigherstandardrwmdisadv { get; set; }
        public decimal? SipProgress8score { get; set; }
        public decimal? SipProgress8scoredisadvantaged { get; set; }
        public Guid? SipParentaccountid { get; set; }
        public decimal? SipReadingprogressscore { get; set; }
        public decimal? SipReadingprogressscoredisadv { get; set; }
        public decimal? SipWritingprogressscoredisadv { get; set; }
        public decimal? SipWritingprogressscore { get; set; }
        public decimal? SipMathsprogressscore { get; set; }
        public decimal? SipMathsprogressscoredisadv { get; set; }
        public decimal? SipAttainment8score { get; set; }
        public decimal? SipAttainment8scoredisadvantaged { get; set; }
        public decimal? SipAttainment8scoreenglish { get; set; }
        public decimal? SipAttainment8scoreenglishdisadvantaged { get; set; }
        public decimal? SipAttainment8scoremaths { get; set; }
        public decimal? SipAttainment8scoremathsdisadvantaged { get; set; }
        public decimal? SipAttainment8scoreebacc { get; set; }
        public decimal? SipAttainment8scoreebaccdisadvantaged { get; set; }
        public int? SipNumberofpupilsprogress8 { get; set; }
        public int? SipNumberofpupilsprogress8disadvantaged { get; set; }
        public decimal? SipProgress8upperconfidence { get; set; }
        public decimal? SipProgress8lowerconfidence { get; set; }
        public decimal? SipProgress8english { get; set; }
        public decimal? SipProgress8englishdisadvantaged { get; set; }
        public decimal? SipProgress8maths { get; set; }
        public decimal? SipProgress8mathsdisadvantaged { get; set; }
        public decimal? SipProgress8ebacc { get; set; }
        public decimal? SipProgress8ebaccdisadvantaged { get; set; }
        public int? SipPerformancetype { get; set; }
        public decimal? SipAppliedGeneralAveragePspe { get; set; }
        public decimal? SipAcademicLevelAveragePspe { get; set; }
        public string SipLocalauthoritycode { get; set; }
        public decimal? SipEnteringEbacc { get; set; }
        public decimal? SipEnteringEbaccEngland { get; set; }
        public decimal? SipEnteringEbaccLocalAuthorityAverage { get; set; }
        public decimal? SipAppliedGeneralProgress { get; set; }
        public decimal? SipAppliedGeneralProgressDisadvantaged { get; set; }
        public decimal? SipAcademicProgress { get; set; }
        public decimal? SipAcademicProgressDisadvantaged { get; set; }
    }
}

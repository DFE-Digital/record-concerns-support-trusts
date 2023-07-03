import { EnvUsername } from "cypress/constants/cypressConstants";
import { CreateCaseRequest } from "./apiDomain";
import { getUkLocalDate } from "cypress/support/formatDate";

export class CaseBuilder
{
    public static buildOpenCase(): CreateCaseRequest
    {
        const currentDate = getUkLocalDate();

        const result: CreateCaseRequest =
        {
            createdAt: currentDate,
            reviewAt: currentDate,
            createdBy: Cypress.env(EnvUsername),
            trustUkprn: this.chooseRandomTrust(),
            deEscalation: currentDate,
            issue: "test",
            currentStatus: "current status",
            caseAim: "case aim",
            deEscalationPoint: "de-escalation point",
            nextSteps: "next steps",
            caseHistory: "case history",
            statusId: 1,
            ratingId: 4,
            territory: 1
        };

        return result;
    }

    private static chooseRandomTrust()
    {
        let searchTerm = [
            "10058372",
            "10060045",
            "10060278",
            "10058372",
            "10059732",
            "10060186",
            "10080822",
            "10081341",
            "10058833",
            "10058354",
            "10066108",
            "10058598",
            "10059919",
            "10057355",
            "10058295",
            "10059877",
            "10060927",
            "10059550",
            "10058417",
            "10059171",
            "10060716",
            "10060832",
            "10066116",
            "10058998",
            "10058772",
            "10059020",
            "10058154",
            "10059577",
            "10059981",
            "10058198",
            "10060069",
            "10059834",
            "10064323",
            "10060619",
            "10058893",
            "10058873",
            "10060447",
            "10057945",
            "10058340",
            "10058890",
            "10059880",
            "10060445",
            "10058715",
            "10059448",
            "10060131",
            "10058725",
            "10058630",
            "10060260",
            "10058560",
            "10058776",
            "10059501",
            "10058240",
            "10059063",
            "10059055",
            "10060233",
            "10058723",
            "10059998",
            "10058813",
            "10059324",
            "10058181",
            "10061208",
            "10060877",
            "10058468",
            "10064307",
        ];
    
        let trustIndex = Math.floor(Math.random() * searchTerm.length);
        let result = searchTerm[trustIndex];

        return result;
    }
}
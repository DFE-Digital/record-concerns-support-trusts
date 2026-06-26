const axios = require('axios');
const ZapClient = require('zaproxy');

const RISK_LEVELS = ['High', 'Medium', 'Low', 'Informational'];

const RISK_LEVEL_STYLES = {
    High: { textColor: 'Attention', containerStyle: 'attention' },
    Medium: { textColor: 'Warning', containerStyle: 'warning' },
    Low: { containerStyle: 'emphasis' },
    Informational: { textColor: 'Accent', containerStyle: 'accent' },
};

function createColouredTextBlock(text, color, weight) {
    return {
        type: 'TextBlock',
        text,
        wrap: true,
        ...(color ? { color } : {}),
        ...(weight ? { weight } : {}),
    };
}

function buildRiskLevelRow(level, count) {
    const { textColor, containerStyle } = RISK_LEVEL_STYLES[level];

    return {
        type: 'Container',
        style: containerStyle,
        separator: true,
        items: [
            {
                type: 'ColumnSet',
                columns: [
                    {
                        type: 'Column',
                        width: 'stretch',
                        items: [createColouredTextBlock(level, textColor, 'bolder')],
                    },
                    {
                        type: 'Column',
                        width: 'stretch',
                        items: [createColouredTextBlock(count, textColor, 'bolder')],
                    },
                ],
            },
        ],
    };
}

function createZapClient() {
    return new ZapClient({
        apiKey: process.env.ZAP_API_KEY,
        proxy: {
            host: process.env.ZAP_ADDRESS,
            port: process.env.ZAP_PORT,
        },
    });
}

async function fetchScanResults() {
    const zaproxy = createZapClient();
    const summaryResponse = await zaproxy.core.alertsSummary({});
    return summaryResponse.alertsSummary ?? {};
}

function getAlertCount(summary, level) {
    return Number.parseInt(summary[level] ?? '0', 10);
}

function buildSummaryTable(summary) {
    return [
        {
            type: 'TextBlock',
            text: 'Record Concerns Support Trusts Security Scan Summary',
            weight: 'bolder',
            size: 'medium',
        },
        {
            type: 'ColumnSet',
            columns: [
                {
                    type: 'Column',
                    width: 'stretch',
                    items: [{ type: 'TextBlock', text: 'Risk Level', weight: 'bolder' }],
                },
                {
                    type: 'Column',
                    width: 'stretch',
                    items: [{ type: 'TextBlock', text: 'Number of Alerts', weight: 'bolder' }],
                },
            ],
        },
        ...RISK_LEVELS.map((level) => buildRiskLevelRow(level, getAlertCount(summary, level).toString())),
    ];
}

function buildLinks() {
    const links = [];

    if (process.env.REPORT_LINK) {
        links.push({
            type: 'TextBlock',
            wrap: true,
            text: '**Full report:** [' + process.env.REPORT_LINK + '](' + process.env.REPORT_LINK + ')',
            spacing: 'medium',
        });
    }

    if (process.env.INFORMATION_LINK) {
        links.push({
            type: 'TextBlock',
            wrap: true,
            text: '**Workflow run:** [' + process.env.INFORMATION_LINK + '](' + process.env.INFORMATION_LINK + ')',
            spacing: 'medium',
        });
    }

    return links;
}

function createTeamsMessage(cardBody) {
    return {
        type: 'message',
        attachments: [
            {
                contentType: 'application/vnd.microsoft.card.adaptive',
                contentUrl: null,
                content: {
                    $schema: 'https://adaptivecards.io/schemas/adaptive-card.json',
                    type: 'AdaptiveCard',
                    version: '1.2',
                    body: cardBody,
                },
            },
        ],
    };
}

async function sendZapTeamsNotification() {
    try {
        const summary = await fetchScanResults();
        const cardBody = [...buildSummaryTable(summary), ...buildLinks()];
        const message = createTeamsMessage(cardBody);

        await axios.post(process.env.TEAMS_OWASP_SCAN_WEBHOOK_URL, message);
    } catch {
        process.exit(1);
    }
}

sendZapTeamsNotification();

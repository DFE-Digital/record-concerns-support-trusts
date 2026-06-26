const ZapClient = require('zaproxy');

const PROXY_ENV_VARS = ['HTTP_PROXY', 'HTTPS_PROXY', 'http_proxy', 'https_proxy'];

function withoutHttpProxyEnv() {
    const saved = {};
    for (const key of PROXY_ENV_VARS) {
        saved[key] = process.env[key];
        delete process.env[key];
    }
    return saved;
}

function restoreHttpProxyEnv(saved) {
    for (const key of PROXY_ENV_VARS) {
        if (saved[key] === undefined) {
            delete process.env[key];
        } else {
            process.env[key] = saved[key];
        }
    }
}

function createZapClient() {
    return new ZapClient({
        apiKey: process.env.ZAP_API_KEY,
        proxy: {
            host: process.env.ZAP_ADDRESS,
            port: parseInt(process.env.ZAP_PORT, 10),
        },
    });
}

module.exports = {
    generateZapReport: async () => {
        console.log('Generating ZAP report');

        const savedProxyEnv = withoutHttpProxyEnv();
        try {
            const zaproxy = createZapClient();

            let recordsRemaining = 1;
            while (recordsRemaining > 0) {
                const resp = await zaproxy.pscan.recordsToScan();
                recordsRemaining = parseInt(resp.recordsToScan, 10);
                if (Number.isNaN(recordsRemaining)) {
                    throw new Error(`Unexpected recordsToScan response: ${JSON.stringify(resp)}`);
                }
            }

            const resp = await zaproxy.reports.generate({
                title: 'Report',
                template: 'traditional-html',
                reportfilename: 'ZAP-Report.html',
                reportdir: '/zap/wrk',
            });
            console.log(JSON.stringify(resp));
        } finally {
            restoreHttpProxyEnv(savedProxyEnv);
        }
    },
};

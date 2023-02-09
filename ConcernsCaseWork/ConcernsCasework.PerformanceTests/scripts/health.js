import http from 'k6/http';
import { check, sleep } from 'k6';
import getConfig from '../getConfig.js';

export const options = {
    vus: 100,
    duration: '30s',
    // httpDebug: 'full',
};

const config = getConfig();

export default function () {

    const healthResponse = http.get(`${config.url}/health`);

    check(healthResponse, {
        "Health page succeeded": (res) => res.status === 200
    });

    check(healthResponse.status, 200);
    sleep(1);
}

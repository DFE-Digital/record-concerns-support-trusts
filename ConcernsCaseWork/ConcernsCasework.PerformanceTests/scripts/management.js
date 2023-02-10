import http from 'k6/http';
import { check, sleep } from 'k6';
import getConfig from '../getConfig.js';

export const options = {
    vus: 200,
    duration: '30s',
    // httpDebug: 'full',
};

const config = getConfig();


export default function () {
    const managementResponse = http.get(`${config.url}/case/2000095/management`, {
        headers: {
            "cookie": config.cookie
        }
    });

    check(managementResponse, {
        "Mangement page succeeded": (res) => res.status === 200
    });

    sleep(1);
}

process.env['NODE_TLS_REJECT_UNAUTHORIZED'] = 0;

const puppeteer = require('puppeteer');

// Use puppeteer to log in to azure AD
// Currently cypress has a problem loading the login page
module.exports.azureLogin = async function azureLogin(url, username, password) {
    const browser = await puppeteer.launch({
        headless: false,
        args: [
            "--incognito"
          ]
    });

    const [page] = await browser.pages();

    await page.goto(url);

    const submitSelector = "input[type=submit]";
    const usernameSelector = "input[name=loginfmt]";
    const passwordSelector = "input[name=passwd]";
    const timeout = 2000;

    await page.waitForSelector(usernameSelector);
    await page.waitForTimeout(timeout);
    await page.type(usernameSelector, username, { delay: 50 });
    await page.click(submitSelector);

    await page.waitForSelector(passwordSelector);
    await page.waitForTimeout(timeout);
    await page.type(passwordSelector, password, { delay: 50 });
    await page.click(submitSelector);

    // // Stay signed in
    await page.waitForTimeout(timeout);
    await page.click(submitSelector);

    await page.waitForSelector(".moj-header__logo");

    const cookies = await page.cookies();

    const loginCookie = cookies.find(c => c.name === ".ConcernsCasework.Login");

    await browser.close();

    return loginCookie.value;
}
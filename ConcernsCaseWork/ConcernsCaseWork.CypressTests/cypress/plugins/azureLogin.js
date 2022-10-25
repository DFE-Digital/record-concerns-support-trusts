const puppeteer = require('puppeteer')

// Use puppeteer to log in to azure AD
// Currently cypress has a problem loading the login page
module.exports.azureLogin = async function azureLogin() {
    const browser = await puppeteer.launch({
        headless: false,
    });

    const page = await browser.newPage();

    await page.goto('https://localhost:44387');

    await page.waitForSelector(".moj-header__logo");

    const cookies = await page.cookies();

    const loginCookie = cookies.find(c => c.name === ".ConcernsCasework.Login");

    await browser.close();

    return loginCookie.value;
}
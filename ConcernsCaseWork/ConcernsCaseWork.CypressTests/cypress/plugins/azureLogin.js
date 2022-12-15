import { join } from "path";

process.env['NODE_TLS_REJECT_UNAUTHORIZED'] = 0;

const puppeteer = require('puppeteer');

// Use puppeteer to log in to azure AD
// Currently cypress has a problem loading the login page
module.exports.azureLogin = async function azureLogin(url, username, password) {
    console.log("Launching browser");
    const browser = await puppeteer.launch({
        headless: false,
        ignoreHTTPSErrors: true,
        args: [
            "--incognito"
          ]
    });

    const usernameExists = username.length > 0;
    const passwordExists = password.length > 0;
    console.log("Username exists " + usernameExists);
    console.log("Password exists " + passwordExists);


    const [page] = await browser.pages();

    console.log("Navigating to Azure AD")
    await page.goto(url);

    await page.waitForTimeout(10000);

    await page.screenshot(
    {
        path: join(__dirname, "../screenshots/capture.jpg")
    });

    const submitSelector = "input[type=submit]";
    const usernameSelector = "input[name=loginfmt]";
    const passwordSelector = "input[name=passwd]";
    const timeout = 2000;

    await page.waitForSelector(usernameSelector, { visible: true });
    await page.waitForTimeout(timeout);

    await page.type(usernameSelector, username, { delay: 50 });
    await page.click(submitSelector);

    console.log("Entering password");
    await page.waitForSelector(passwordSelector);
    await page.waitForTimeout(timeout);
    await page.type(passwordSelector, password, { delay: 50 });
    await page.click(submitSelector);

    console.log("Selecting stay signed in")
    // // Stay signed in
    await page.waitForTimeout(timeout);
    await page.click(submitSelector);

    console.log("Waiting for website header");
    await page.waitForSelector(".moj-header__logo");

    console.log("Storing cookie")
    const cookies = await page.cookies();

    const loginCookie = cookies.find(c => c.name === ".ConcernsCasework.Login");

    console.log("Closing browser");
    await browser.close();

    console.log("Returning cookie!");
    return loginCookie.value;
}
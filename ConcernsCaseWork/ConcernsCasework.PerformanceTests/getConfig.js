export default function getConfig()
{
    const fileContents = open("../config.json");
    const config = JSON.parse(fileContents);

    return config;
}
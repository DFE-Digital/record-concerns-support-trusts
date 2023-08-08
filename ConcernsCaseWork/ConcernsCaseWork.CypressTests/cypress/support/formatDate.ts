import { DateTime } from 'luxon';

export function toDisplayDate(date: Date): string {
    return date.toLocaleDateString('en-gb', {day:"numeric", month: "long", year: "numeric"});
}

export function getUkLocalDateFormatted(): string {
    const result = getUkLocalDate();

    return <string>result.toISO();
}

export function getUkLocalDate(): DateTime {
    // Dates are stored in local time in the app
    // Cases are sorted by date, the timezone matters
    // In the future, the app should use ISO
    const result = DateTime.local().setZone('Europe/London');

    return result;
}
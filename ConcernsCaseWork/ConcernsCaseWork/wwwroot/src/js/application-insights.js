// App Insights frontend
// The SDK instance itself is created in _Layout.cshtml by the ASP.NET Core
// JavaScriptSnippet, which only renders when the user has consented to cookies
// and ApplicationInsights:BrowserAnalyticsEnabled is true. We therefore reuse
// window.appInsights here rather than loading a second instance.

// Track clicks on GOV.UK details components (by CSS class)
const trackedClasses = ['govuk-details__summary-text'];
const trackedSelector = trackedClasses.map(c => '.' + c).join(',');

document.addEventListener('click', function (e) {
	if (!(e.target instanceof Element)) return;

	// No instance means analytics is disabled or the user has not consented
	if (!window.appInsights || typeof window.appInsights.trackEvent !== 'function') return;

	// Keyboard activation (Enter/Space) targets the <summary>, not the inner text span
	const summary = e.target.closest('.govuk-details__summary');
	const el = e.target.closest(trackedSelector) || (summary && summary.querySelector(trackedSelector));
	if (!el) return;

	const details = el.closest('details');
	window.appInsights.trackEvent({
		name: 'Accordion click event',
		properties: {
			className: trackedClasses.find(c => el.classList.contains(c)),
			text: el.innerText ? el.innerText.trim() : undefined,
			page: window.location.pathname,
			// click fires before the toggle, so the new state is the opposite of the current one
			action: details ? (details.open ? 'collapse' : 'expand') : undefined
		}
	});
});

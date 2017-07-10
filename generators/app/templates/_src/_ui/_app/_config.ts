
const baseUri = 'https://localhost:<%=port%>';

export default {

    uri: {
        auth: baseUri + '/auth/',
        site: baseUri,
        services: baseUri + "/api/"
    },
    auth: {
        accessTokenKey: 'AUTH-LOCAL',
        externalProviderKey: 'AUTH-EXTERNAL'
    },
    uopt: 'UOPT',
    xsrf: {
        cookieName: 'XSRF-TOKEN',
        headerName: 'X-XSRF-TOKEN'
    }
};
window.Downloadhelper = {
    downloadWithJwt: async function (url, token, filename) {
        const response = await fetch(url, {
            headers: { Authorization: `Bearer ${token}` }
        });
        if (!response.ok) throw new Error("Download failed");
        const blob = await response.blob();
        const link = document.createElement("a");
        link.href = URL.createObjectURL(blob);
        link.download = filename;
        link.click();
        URL.revokeObjectURL(link.href);
    }
};


window.blazorHelpers = {
    getWidth: function () {
        return window.innerWidth;
    },
    registerResizeCallback: function (dotNetObjRef) {
        window.addEventListener("resize", () => {
            dotNetObjRef.invokeMethodAsync("OnBrowserResize", window.innerWidth);
        });
    }
};
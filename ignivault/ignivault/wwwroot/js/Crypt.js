window.Crypt = {
    // --- Random bytes ---
    generateRandomKeyBase64: function () {
        const key = new Uint8Array(32); // 32 bytes = 256 bits
        crypto.getRandomValues(key);
        return this._uint8ArrayToBase64(key);
    },

    generateSaltBase64: function () {
        const salt = new Uint8Array(16);
        crypto.getRandomValues(salt);
        return this._uint8ArrayToBase64(salt);
    },

    // --- Derive key from password + salt ---
    deriveKeyBase64: async function (password, saltBase64) {
        const enc = new TextEncoder();
        const salt = this._base64ToUint8Array(saltBase64);

        const keyMaterial = await crypto.subtle.importKey(
            "raw",
            enc.encode(password),
            "PBKDF2",
            false,
            ["deriveKey"]
        );

        const key = await crypto.subtle.deriveKey(
            { name: "PBKDF2", salt: salt, iterations: 100000, hash: "SHA-256" },
            keyMaterial,
            { name: "AES-CBC", length: 256 },
            true,
            ["encrypt", "decrypt"]
        );

        const rawKey = await crypto.subtle.exportKey("raw", key);
        return this._uint8ArrayToBase64(new Uint8Array(rawKey));
    },

    // --- AES Encrypt ---
    encryptBase64: async function (plaintextBase64, keyBase64) {
        const plaintext = this._base64ToUint8Array(plaintextBase64);
        const keyBytes = this._base64ToUint8Array(keyBase64);
        const iv = crypto.getRandomValues(new Uint8Array(16));

        const key = await crypto.subtle.importKey(
            "raw",
            keyBytes,
            { name: "AES-CBC" },
            false,
            ["encrypt"]
        );

        const ciphertext = await crypto.subtle.encrypt(
            { name: "AES-CBC", iv: iv },
            key,
            plaintext
        );

        return {
            ciphertext: this._uint8ArrayToBase64(new Uint8Array(ciphertext)),
            iv: this._uint8ArrayToBase64(iv)
        };
    },

    // --- AES Decrypt ---
    decryptBase64: async function (ciphertextBase64, keyBase64, ivBase64) {
        const ciphertext = this._base64ToUint8Array(ciphertextBase64);
        const keyBytes = this._base64ToUint8Array(keyBase64);
        const iv = this._base64ToUint8Array(ivBase64);

        const key = await crypto.subtle.importKey(
            "raw",
            keyBytes,
            { name: "AES-CBC" },
            false,
            ["decrypt"]
        );

        const decrypted = await crypto.subtle.decrypt(
            { name: "AES-CBC", iv: iv },
            key,
            ciphertext
        );

        return this._uint8ArrayToBase64(new Uint8Array(decrypted));
    },

    // --- Encrypt Master Key ---
    encryptMasterKeyBase64: async function (masterKeyBase64, password, saltBase64) {
        const masterKey = this._base64ToUint8Array(masterKeyBase64);
        const salt = this._base64ToUint8Array(saltBase64);
        const passwordKeyBase64 = await this.deriveKeyBase64(password, saltBase64);
        const result = await this.encryptBase64(this._uint8ArrayToBase64(masterKey), passwordKeyBase64);
        return result; // { ciphertext: Base64, iv: Base64 }
    },

    // --- Decrypt Master Key ---
    decryptMasterKeyBase64: async function (encryptedMasterKeyBase64, password, saltBase64, ivBase64) {
        const passwordKeyBase64 = await this.deriveKeyBase64(password, saltBase64);
        const decryptedBase64 = await this.decryptBase64(encryptedMasterKeyBase64, passwordKeyBase64, ivBase64);
        return decryptedBase64; // Base64 string of master key
    },

    // --- Helpers ---
    _uint8ArrayToBase64: function (uint8Array) {
        let binary = '';
        uint8Array.forEach(b => binary += String.fromCharCode(b));
        return btoa(binary);
    },

    _base64ToUint8Array: function (base64) {
        const binary = atob(base64);
        const array = new Uint8Array(binary.length);
        for (let i = 0; i < binary.length; i++) {
            array[i] = binary.charCodeAt(i);
        }
        return array;
    }
};

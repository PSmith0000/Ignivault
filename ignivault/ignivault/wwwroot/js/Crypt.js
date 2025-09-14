window.Crypt = {
    // --- Random bytes ---
    generateRandomKey: function () {
        const key = new Uint8Array(32); // 256-bit key
        crypto.getRandomValues(key);
        return key; // Uint8Array
    },

    generateSalt: function () {
        const salt = new Uint8Array(16); // 128-bit salt
        crypto.getRandomValues(salt);
        return salt; // Uint8Array
    },

    // --- Derive key from password + salt ---
    deriveKey: async function (password, salt) {
        // salt is Uint8Array
        const enc = new TextEncoder();
        const keyMaterial = await crypto.subtle.importKey(
            "raw",
            enc.encode(password),
            "PBKDF2",
            false,
            ["deriveKey"]
        );

        const key = await crypto.subtle.deriveKey(
            { name: "PBKDF2", salt: salt, iterations: 100_000, hash: "SHA-256" },
            keyMaterial,
            { name: "AES-CBC", length: 256 },
            true,
            ["encrypt", "decrypt"]
        );

        const rawKey = await crypto.subtle.exportKey("raw", key);
        return new Uint8Array(rawKey); // raw key bytes
    },

    // --- AES Encrypt ---
    encrypt: async function (plaintext, keyBytes) {
        // plaintext and keyBytes are Uint8Array
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
            ciphertext: new Uint8Array(ciphertext),
            iv: iv
        };
    },

    decrypt: async function (ciphertext, keyBytes, iv) {
        // all arguments are Uint8Array
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

        return new Uint8Array(decrypted);
    },

    // --- Encrypt Master Key ---
    encryptMasterKey: async function (masterKeyBytes, password, salt) {
        const passwordKey = await this.deriveKey(password, salt);
        const result = await this.encrypt(masterKeyBytes, passwordKey);
        return result; // { ciphertext: Uint8Array, iv: Uint8Array }
    },

    // --- Decrypt Master Key ---
    decryptMasterKey: async function (encryptedMasterKeyBytes, password, salt, iv) {
        const passwordKey = await this.deriveKey(password, salt);
        const decrypted = await this.decrypt(encryptedMasterKeyBytes, passwordKey, iv);
        return decrypted; // Uint8Array
    },
};

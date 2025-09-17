window.Crypt = {
    // --- Random bytes -------------------------------------------------------
    generateRandomKey() {
        // 256-bit AES key
        const key = new Uint8Array(32);
        crypto.getRandomValues(key);
        return key;
    },

    generateSalt() {
        // 128-bit salt
        const salt = new Uint8Array(16);
        crypto.getRandomValues(salt);
        return salt;
    },

    // --- Derive key from password + salt ------------------------------------
    async deriveKey(password, salt) {
        // salt: Uint8Array
        const enc = new TextEncoder();
        const keyMaterial = await crypto.subtle.importKey(
            "raw",
            enc.encode(password),
            "PBKDF2",
            false,
            ["deriveKey"]
        );

        const key = await crypto.subtle.deriveKey(
            {
                name: "PBKDF2",
                salt,
                iterations: 100_000,
                hash: "SHA-256"
            },
            keyMaterial,
            { name: "AES-CBC", length: 256 },
            true,
            ["encrypt", "decrypt"]
        );

        return new Uint8Array(await crypto.subtle.exportKey("raw", key));
    },

    // --- AES-CBC Encrypt/Decrypt -------------------------------------------
    async encrypt(plaintextBytes, keyBytes) {
        // plaintextBytes, keyBytes: Uint8Array
        const iv = crypto.getRandomValues(new Uint8Array(16));
        const key = await crypto.subtle.importKey("raw", keyBytes, "AES-CBC", false, ["encrypt"]);

        const ciphertext = await crypto.subtle.encrypt(
            { name: "AES-CBC", iv },
            key,
            plaintextBytes
        );

        return { ciphertext: new Uint8Array(ciphertext), iv };
    },

    async decrypt(ciphertextBytes, keyBytes, iv) {
        const key = await crypto.subtle.importKey("raw", keyBytes, "AES-CBC", false, ["decrypt"]);

        const plaintext = await crypto.subtle.decrypt(
            { name: "AES-CBC", iv },
            key,
            ciphertextBytes
        );

        return new Uint8Array(plaintext);
    },

    // --- Master-key helpers -------------------------------------------------
    async encryptMasterKey(masterKeyBytes, password, salt) {
        const pwdKey = await this.deriveKey(password, salt);
        return this.encrypt(masterKeyBytes, pwdKey);
    },

    async decryptMasterKey(encryptedMasterKeyBytes, password, salt, iv) {
        const pwdKey = await this.deriveKey(password, salt);
        return this.decrypt(encryptedMasterKeyBytes, pwdKey, iv);
    }
};
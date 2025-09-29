// Helper function to convert an ArrayBuffer or Uint8Array to a Base64 string
function toBase64(buffer) {
    let binary = '';
    const bytes = new Uint8Array(buffer);
    const len = bytes.byteLength;
    for (let i = 0; i < len; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    return window.btoa(binary);
}

// Derives a cryptographic key from a master password and salt using PBKDF2.
async function deriveKey(password, salt) {
    const enc = new TextEncoder();
    const keyMaterial = await window.crypto.subtle.importKey(
        "raw",
        enc.encode(password),
        { name: "PBKDF2" },
        false,
        ["deriveKey"]
    );
    return await window.crypto.subtle.deriveKey(
        {
            name: "PBKDF2",
            salt: salt,
            iterations: 250000,
            hash: "SHA-256"
        },
        keyMaterial,
        { name: "AES-GCM", length: 256 },
        true,
        ["encrypt", "decrypt"]
    );
}

/**
 * Encrypts data using the derived key.
 * @returns {object} An object containing the Base64-encoded encrypted data and IV.
 */
window.encryptData = async (data, iv, salt, password) => {
    try {
        const key = await deriveKey(password, salt);
        const encryptedContent = await window.crypto.subtle.encrypt(
            { name: "AES-GCM", iv: iv },
            key,
            data
        );

        // Return Base64 encoded strings for reliable transfer to C#
        return {
            encryptedData: toBase64(encryptedContent),
            iv: toBase64(iv)
        };
    } catch (e) {
        console.error("Encryption failed:", e);
        return null;
    }
};

/**
 * Decrypts data using the derived key.
 * @returns {string} The Base64-encoded decrypted content.
 */
window.decryptData = async (encryptedData, iv, salt, password) => {
    try {
        const key = await deriveKey(password, salt);
        const decryptedContent = await window.crypto.subtle.decrypt(
            { name: "AES-GCM", iv: iv },
            key,
            encryptedData
        );
        // FIX: Return the raw decrypted bytes directly.
        return new Uint8Array(decryptedContent);
    } catch (e) {
        console.error("Decryption failed:", e);
        return null;
    }
};
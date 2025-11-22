export async function uploadMeterReadings(data) {
    const formData = new FormData();
    formData.append("file", data);

    const response = await fetch("http://localhost:5297/api/meter-readings-upload", {
        method: "POST",        
        body: formData
    });

    if (!response.ok) {
        const error = await response.json();
        throw new Error(error.title || "Upload failed");
    }

    return await response.json();
}

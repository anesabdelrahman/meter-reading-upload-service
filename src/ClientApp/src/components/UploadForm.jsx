import React, { useState } from "react";
import { Card, CardContent } from "../components/ui/card";
import { Button } from "../components/ui/button";
import { motion } from "framer-motion";

export default function UploadForm({ onSuccess }) {
    const [file, setFile] = useState(null);
    const [error, setError] = useState(null);
    const [isLoading, setIsLoading] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError(null);

        if (!file) {
            setError("Please select a CSV file.");
            return;
        }

        const formData = new FormData();
        formData.append("file", file);

        try {
            setIsLoading(true);
            const response = await fetch("http://localhost:5297/api/meter-readings-upload", {
                method: "POST",
                body: formData,
            });

            if (!response.ok) {
                const message = await response.text();
                throw new Error(message || "Upload failed.");
            }

            const data = await response.json();
            onSuccess(data);
        } catch (err) {
            setError(err.message);
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <Card className="p-6 shadow-soft rounded-2xl">
            <CardContent>
                <h2 className="text-xl font-semibold mb-4">Upload CSV File</h2>

                <form onSubmit={handleSubmit} className="space-y-4">
                    <label className="flex flex-col sm:flex-row items-center gap-3 bg-background/60 p-3 rounded-lg border border-input">
                        <input
                            type="file"
                            accept=".csv"
                            onChange={(e) => setFile(e.target.files[0])}
                            className="sr-only"
                        />

                        <div className="flex-1 text-sm text-muted-foreground">
                            {file ? (
                                <div className="flex items-center justify-between gap-3">
                                    <span className="truncate">{file.name}</span>
                                    <span className="text-xs text-muted-foreground">{Math.max(0, Math.round((file.size/1024))) } KB</span>
                                </div>
                            ) : (
                                <span className="text-sm">Choose a CSV file to upload</span>
                            )}
                        </div>

                        <Button
                            asChild={false}
                            type="button"
                            onClick={(e) => {
                                // trigger the hidden input
                                const input = e.currentTarget.closest('label')?.querySelector('input[type=file]');
                                input?.click();
                            }}
                            variant="outline"
                            className="whitespace-nowrap"
                        >
                            Browse
                        </Button>
                    </label>

                    <Button type="submit" disabled={isLoading} className="w-full rounded-xl">
                        {isLoading ? "Uploading…" : "Upload"}
                    </Button>
                </form>

                {error && (
                    <motion.div
                        initial={{ opacity: 0 }}
                        animate={{ opacity: 1 }}
                        className="mt-4 p-3 rounded bg-destructive/10 text-destructive"
                    >
                        {error}
                    </motion.div>
                )}
            </CardContent>
        </Card>
    );
}

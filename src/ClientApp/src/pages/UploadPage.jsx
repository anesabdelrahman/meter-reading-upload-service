import { useState } from "react";
import UploadForm from "../components/UploadForm";
import ResultsCard from "../components/ResultsCard";

export default function UploadPage() {
    const [result, setResult] = useState(null);

    return (
        <div className="max-w-3xl mx-auto mt-12 space-y-8">
            <h1 className="text-3xl font-bold text-center">Upload Meter Readings</h1>

            <UploadForm onSuccess={setResult} />

            {result && <ResultsCard result={result} />}
        </div>
    );
}

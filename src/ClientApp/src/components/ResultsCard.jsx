import { Card, CardContent } from "../components/ui/card";
import { motion } from "framer-motion";

export default function ResultsCard({ result }) {
    return (
        <motion.div
            initial={{ opacity: 0, y: 12 }}
            animate={{ opacity: 1, y: 0 }}
        >
            <Card className="p-6 shadow-medium rounded-2xl bg-green-50 border-green-200">
                <CardContent className="space-y-4 text-green-900">
                    <h2 className="text-xl font-bold">Upload Summary</h2>

                    <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 text-center">
                        <div className="p-4 bg-white rounded shadow-soft flex flex-col items-center">
                            <p className="text-sm font-semibold text-muted-foreground">
                                Success
                            </p>
                            <p className="text-3xl font-extrabold text-green-700">
                                {result.success}
                            </p>
                        </div>

                        <div className="p-4 bg-white rounded shadow-soft flex flex-col items-center">
                            <p className="text-sm font-semibold text-muted-foreground">
                                Failed
                            </p>
                            <p className="text-3xl font-extrabold text-red-600">
                                {result.failed}
                            </p>
                        </div>

                        <div className="p-4 bg-white rounded shadow-soft flex flex-col items-center">
                            <p className="text-sm font-semibold text-muted-foreground">
                                Errors
                            </p>
                            <p className="text-3xl font-extrabold text-amber-600">
                                {result.errors?.length ?? 0}
                            </p>
                        </div>
                    </div>

                    {result.errors?.length > 0 && (
                        <div className="mt-4">
                            <p className="font-semibold mb-2">Error Details:</p>
                            <ul className="list-disc ml-5 space-y-1">
                                {result.errors.map((err, idx) => (
                                    <li key={idx}>{err}</li>
                                ))}
                            </ul>
                        </div>
                    )}
                </CardContent>
            </Card>
        </motion.div>
    );
}

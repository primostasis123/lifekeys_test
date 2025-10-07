"use client";
import { useEffect, useState } from "react";
import type { Checkin } from "../app/page";
import { useRouter } from "next/navigation";

type Props = {
  selected: Checkin | null;
  onSaved?: (saved: Checkin) => void;
  onCancelEdit?: () => void;
};

export default function Checkins({ selected, onSaved, onCancelEdit }: Props) {
  const [mood, setMood] = useState<number>(1);
  const [notes, setNotes] = useState("");
  const [saving, setSaving] = useState(false);
  const apiUrl = `${process.env.NEXT_PUBLIC_API_URL}/checkins`;
  const router = useRouter()
  // Prefill form when editing
  useEffect(() => {
    if (selected) {
      setMood(selected.mood);
      setNotes(selected.notes ?? "");
    } else {
      setMood(1);
      setNotes("");
    }
  }, [selected]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);

    try {
      const token = localStorage.getItem("authToken");
      if (!token) {
        router.push("/login");
        return;
      }

      const isEdit = !!selected?.id;
      const url = isEdit ? `${apiUrl}/${selected!.id}` : apiUrl;
      const method = isEdit ? "PUT" : "POST";

      const res = await fetch(url, {
        method,
        headers: {
          "Content-Type": "application/json",
          Authorization: `Basic ${token}`,
        },
        body: JSON.stringify({ mood, notes }),
      });

      if (!res.ok) throw new Error("Failed to save check-in");
      const saved = await res.json();

      // Reset when creating; stay filled when editing until cancel or switch
      if (!isEdit) {
        setMood(1);
        setNotes("");
      }

      onSaved?.(saved);
      alert(isEdit ? "Check-in updated successfully" : "Check-in submitted successfully");
    } catch (err) {
      console.error(err);
      alert("Failed to save check-in");
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="max-w-2xl mx-auto">
      <h2 className="text-3xl font-bold text-slate-900 mb-6">
        {selected ? "Update Check-in" : "Daily Check-in"}
      </h2>

      <div className="bg-white rounded-xl shadow-sm p-8 border border-slate-200">
        <form onSubmit={handleSubmit} className="space-y-6">
          <div>
            <label className="block text-lg font-semibold text-slate-900 mb-4">
              How are you feeling today?
            </label>
            <div className="flex items-center gap-4 justify-between">
              {[1, 2, 3, 4, 5].map((value) => (
                <button
                  key={value}
                  type="button"
                  onClick={() => setMood(value)}
                  className={`flex-1 py-4 rounded-lg font-semibold text-lg transition-all transform hover:scale-105 ${
                    mood === value
                      ? "bg-slate-900 text-white shadow-lg"
                      : "bg-slate-100 text-slate-600 hover:bg-slate-200"
                  }`}
                >
                  {value}
                </button>
              ))}
            </div>
          </div>

          <div>
            <label htmlFor="notes" className="block text-lg font-semibold text-slate-900 mb-2">
              Notes (Optional)
            </label>
            <textarea
              id="notes"
              value={notes}
              onChange={(e) => setNotes(e.target.value)}
              rows={6}
              className="w-full px-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-slate-900 focus:border-transparent outline-none resize-none"
              placeholder="Share what's on your mind..."
            />
          </div>

          <div className="flex gap-3">
            <button
              type="submit"
              disabled={saving}
              className="w-full bg-slate-900 text-white py-3 px-6 rounded-lg font-semibold hover:bg-slate-800 transition-all disabled:opacity-60"
            >
              {selected ? "Update Check-in" : "Submit Check-in"}
            </button>

            {selected && (
              <button
                type="button"
                onClick={onCancelEdit}
                className="w-full bg-slate-100 text-slate-900 py-3 px-6 rounded-lg font-semibold hover:bg-slate-200 transition-all"
              >
                Cancel
              </button>
            )}
          </div>
        </form>
      </div>
    </div>
  );
}

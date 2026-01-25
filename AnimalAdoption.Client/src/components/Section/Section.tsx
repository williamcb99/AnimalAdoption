import "./Section.module.css";

export const Section = ({ className, children }: { className?: string; children?: React.ReactNode }) => {
    return (
        <section className={className}>
            {children}
        </section>
    );
}